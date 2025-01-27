using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services.Factories;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static MTGApplication.Features.DeckEditor.Editor.UseCases.DeckEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;
public partial class DeckEditorViewModel : ObservableObject, ISavable, IWorker
{
  public DeckEditorViewModel(IMTGCardImporter importer, Notifier? notifier = null, DeckEditorConfirmers? confirmers = null)
  {
    Importer = importer;
    Notifier = notifier ?? new();
    Confirmers = confirmers ?? new();

    UndoStack.CollectionChanged += UndoStack_CollectionChanged;
  }

  [NotNull]
  public DeckEditorMTGDeck? Deck
  {
    get => field ??= Deck = new();
    set
    {
      if (field == value)
        return;

      if (field != null)
      {
        field.PropertyChanging -= Deck_PropertyChanging;
        field.PropertyChanged -= Deck_PropertyChanged;
        field.DeckCards.CollectionChanged -= DeckCards_CollectionChanged;

        foreach (var item in field.DeckCards)
          item.PropertyChanged -= DeckCard_PropertyChanged;
      }

      SetProperty(ref field, value);

      if (field != null)
      {
        field.PropertyChanging += Deck_PropertyChanging;
        field.PropertyChanged += Deck_PropertyChanged;
        field.DeckCards.CollectionChanged += DeckCards_CollectionChanged;

        foreach (var item in field.DeckCards)
          item.PropertyChanged += DeckCard_PropertyChanged;
      }

      DeckCardList.Cards = Deck.DeckCards;
      MaybeCardList.Cards = Deck.Maybelist;
      WishCardList.Cards = Deck.Wishlist;
      RemoveCardList.Cards = Deck.Removelist;

      Commander.Card = Deck.Commander;
      Partner.Card = Deck.CommanderPartner;

      OnPropertyChanged(nameof(Name));
      OnPropertyChanged(nameof(Size));
      OnPropertyChanged(nameof(Price));

      SaveDeckCommand?.NotifyCanExecuteChanged();
      DeleteDeckCommand?.NotifyCanExecuteChanged();
      ShowDeckTokensCommand?.NotifyCanExecuteChanged();
      OpenDeckTestingWindowCommand?.NotifyCanExecuteChanged();
    }
  }

  public IMTGCardImporter Importer { get; }
  public DeckEditorConfirmers Confirmers { get; }
  public Notifier Notifier { get; }
  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public ReversibleCommandStack UndoStack { get; } = new();

  [NotNull] public GroupedCardListViewModel? DeckCardList => field ??= new DeckEditorCardListFactory(this).CreateGroupedCardListViewModel(Deck.DeckCards);
  [NotNull] public CardListViewModel? MaybeCardList => field ??= new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Maybelist);
  [NotNull] public CardListViewModel? WishCardList => field ??= new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Wishlist);
  [NotNull] public CardListViewModel? RemoveCardList => field ??= new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Removelist);
  [NotNull]
  public CommanderViewModel? Commander
  {
    get => field ??= Commander = new DeckEditorCommanderFactory(this).CreateCommanderViewModel(Deck.Commander);
    private set
    {
      if (field != null)
        return;

      field = value;

      Commander.PropertyChanged += CommanderViewModel_PropertyChanged;
    }
  }
  [NotNull]
  public CommanderViewModel? Partner
  {
    get => field ??= Partner = new DeckEditorCommanderFactory(this).CreateCommanderViewModel(Deck.CommanderPartner);
    private set
    {
      if (field != null)
        return;

      field = value;

      Partner.PropertyChanged += CommanderViewModel_PropertyChanged;
    }
  }

  [ObservableProperty] public partial bool IsBusy { get; set; }
  [ObservableProperty] public partial bool HasUnsavedChanges { get; set; }

  public string Name
  {
    get => Deck.Name;
    set { if (Name != value) { Deck.Name = value; OnPropertyChanged(); } }
  }
  public int Size => Deck.DeckCards.Sum(x => x.Count) + (Deck.Commander != null ? 1 : 0) + (Deck.CommanderPartner != null ? 1 : 0);
  public double Price => Deck.DeckCards.Sum(x => x.Info.Price * x.Count) + (Deck.Commander?.Info.Price ?? 0) + (Deck.CommanderPartner?.Info.Price ?? 0);

  [NotNull] public IAsyncRelayCommand<ISavable.ConfirmArgs>? ConfirmUnsavedChangesCommand => field ??= new ConfirmUnsavedChanges(this).Command;
  [NotNull] public IAsyncRelayCommand? NewDeckCommand => field ??= new NewDeck(this).Command;
  [NotNull] public IAsyncRelayCommand<string>? OpenDeckCommand => field ??= new OpenDeck(this).Command;
  [NotNull] public IAsyncRelayCommand? SaveDeckCommand => field ??= new SaveDeck(this).Command;
  [NotNull] public IAsyncRelayCommand? DeleteDeckCommand => field ??= new DeleteDeck(this).Command;
  [NotNull] public IRelayCommand? UndoCommand => field ??= new Undo(this).Command;
  [NotNull] public IRelayCommand? RedoCommand => field ??= new Redo(this).Command;
  [NotNull] public IAsyncRelayCommand? OpenEdhrecCommanderWebsiteCommand => field ??= new OpenEdhrecCommanderWebsite(this).Command;
  [NotNull] public IAsyncRelayCommand? ShowDeckTokensCommand => field ??= new ShowDeckTokens(this).Command;
  [NotNull] public IRelayCommand? OpenDeckTestingWindowCommand => field ??= new OpenDeckTestingWindow(this).Command;
  [NotNull] public IRelayCommand? OpenEdhrecSearchWindowCommand => field ??= new OpenEdhrecSearchWindow(this).Command;

  private void DeckCards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is IList newItems)
    {
      foreach (var item in newItems.OfType<DeckEditorMTGCard>())
        item.PropertyChanged += DeckCard_PropertyChanged;
    }
    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is IList oldItems)
    {
      foreach (var item in oldItems.OfType<DeckEditorMTGCard>())
        item.PropertyChanged -= DeckCard_PropertyChanged;
    }

    OnPropertyChanged(nameof(Size));
    OnPropertyChanged(nameof(Price));

    ShowDeckTokensCommand?.NotifyCanExecuteChanged();
    OpenDeckTestingWindowCommand?.NotifyCanExecuteChanged();
  }

  private void DeckCard_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    OnPropertyChanged(nameof(Size));
    OnPropertyChanged(nameof(Price));
  }

  private void CommanderViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(CommanderViewModel.Card))
    {
      if (sender == Commander)
        Deck.Commander = Commander.Card;
      else if (sender == Partner)
        Deck.CommanderPartner = Partner.Card;
    }
  }

  private void Deck_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
  {
    if (e.PropertyName == nameof(DeckEditorMTGDeck.Commander) && Deck.Commander != null)
      Deck.Commander.PropertyChanged -= DeckCard_PropertyChanged;
    else if (e.PropertyName == nameof(DeckEditorMTGDeck.CommanderPartner) && Deck.CommanderPartner != null)
      Deck.CommanderPartner.PropertyChanged -= DeckCard_PropertyChanged;
  }

  private void Deck_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(DeckEditorMTGDeck.Commander))
    {
      if (Deck.Commander != null)
        Deck.Commander.PropertyChanged += DeckCard_PropertyChanged;

      OnPropertyChanged(nameof(Size));
      OnPropertyChanged(nameof(Price));

      ShowDeckTokensCommand.NotifyCanExecuteChanged();
      OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
      OpenEdhrecSearchWindowCommand.NotifyCanExecuteChanged();
    }
    else if (e.PropertyName == nameof(DeckEditorMTGDeck.CommanderPartner))
    {
      if (Deck.CommanderPartner != null)
        Deck.CommanderPartner.PropertyChanged += DeckCard_PropertyChanged;

      OnPropertyChanged(nameof(Size));
      OnPropertyChanged(nameof(Price));

      ShowDeckTokensCommand.NotifyCanExecuteChanged();
      OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
      OpenEdhrecSearchWindowCommand.NotifyCanExecuteChanged();
    }
  }

  private void UndoStack_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    => HasUnsavedChanges = e.Action != NotifyCollectionChangedAction.Reset;
}