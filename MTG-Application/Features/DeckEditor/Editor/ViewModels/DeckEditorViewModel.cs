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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;
using static MTGApplication.Features.DeckEditor.Editor.UseCases.DeckEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;
public partial class DeckEditorViewModel : ObservableObject, ISavable, IWorker
{
  public DeckEditorViewModel(IMTGCardImporter importer)
  {
    Importer = importer;

    UndoStack.CollectionChanged += UndoStack_CollectionChanged;
  }

  [NotNull, DisallowNull]
  public DeckEditorMTGDeck? Deck
  {
    get => field ??= Deck = new();
    set
    {
      ArgumentNullException.ThrowIfNull(value);

      if (field == value)
        return;

      var fieldWasNull = field == null;

      if (field != null)
      {
        field.PropertyChanging -= Deck_PropertyChanging;
        field.PropertyChanged -= Deck_PropertyChanged;
        field.DeckCards.CollectionChanged -= DeckCards_CollectionChanged;
        field.Maybelist.CollectionChanged -= CardList_CollectionChanged;
        field.Wishlist.CollectionChanged -= CardList_CollectionChanged;
        field.Removelist.CollectionChanged -= CardList_CollectionChanged;

        foreach (var item in field.DeckCards)
          item.PropertyChanged -= DeckCard_PropertyChanged;
      }

      SetProperty(ref field, value);

      field.PropertyChanging += Deck_PropertyChanging;
      field.PropertyChanged += Deck_PropertyChanged;
      field.DeckCards.CollectionChanged += DeckCards_CollectionChanged;
      field.Maybelist.CollectionChanged += CardList_CollectionChanged;
      field.Wishlist.CollectionChanged += CardList_CollectionChanged;
      field.Removelist.CollectionChanged += CardList_CollectionChanged;

      foreach (var item in field.DeckCards)
        item.PropertyChanged += DeckCard_PropertyChanged;

      AttachCommandsToCards([
        .. field.DeckCards,
        .. field.Maybelist,
        .. field.Wishlist,
        .. field.Removelist,
        ]);

      AttachCommandsToCards(new List<DeckEditorMTGCard?>([
        field.Commander,
        field.CommanderPartner
        ]).OfType<DeckEditorMTGCard>());

      // Reset cardlist viewmodels
      DeckCardList = null;
      MaybeCardList = null;
      WishCardList = null;
      RemoveCardList = null;

      // Reset commander viewmodels
      Commander = null;
      Partner = null;

      if (!fieldWasNull) // don't init lazy properties if the deck was not already set
      {
        DeleteDeckCommand?.NotifyCanExecuteChanged();
        ShowDeckTokensCommand?.NotifyCanExecuteChanged();
        OpenDeckTestingWindowCommand?.NotifyCanExecuteChanged();
        OpenEdhrecSearchWindowCommand?.NotifyCanExecuteChanged();
        OpenEdhrecCommanderWebsiteCommand?.NotifyCanExecuteChanged();
      }

      OnPropertyChanged(nameof(Name));
      OnPropertyChanged(nameof(Size));
      OnPropertyChanged(nameof(Price));
    }
  }

  public IMTGCardImporter Importer { get; }
  public ReversibleCommandStack UndoStack { get; } = new();
  public DeckEditorConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();

  [NotNull]
  public GroupedCardListViewModel? DeckCardList
  {
    get => field ??= DeckCardList = new DeckEditorCardListFactory(this).CreateGroupedCardListViewModel(Deck.DeckCards);
    private set => SetProperty(ref field, value);
  }
  [NotNull]
  public CardListViewModel? MaybeCardList
  {
    get => field ??= MaybeCardList = new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Maybelist);
    private set => SetProperty(ref field, value);
  }
  [NotNull]
  public CardListViewModel? WishCardList
  {
    get => field ??= WishCardList = new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Wishlist);
    private set => SetProperty(ref field, value);
  }
  [NotNull]
  public CardListViewModel? RemoveCardList
  {
    get => field ??= RemoveCardList = new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Removelist);
    private set => SetProperty(ref field, value);
  }
  [NotNull]
  public CommanderViewModel? Commander
  {
    get => field ??= Commander = new DeckEditorCommanderFactory(this).CreateCommanderViewModel(Deck.Commander);
    private set
    {
      field?.PropertyChanged -= CommanderViewModel_PropertyChanged;
      SetProperty(ref field, value);
      field?.PropertyChanged += CommanderViewModel_PropertyChanged;
    }
  }
  [NotNull]
  public CommanderViewModel? Partner
  {
    get => field ??= Partner = new DeckEditorCommanderFactory(this).CreateCommanderViewModel(Deck.CommanderPartner);
    private set
    {
      field?.PropertyChanged -= CommanderViewModel_PropertyChanged;
      SetProperty(ref field, value);
      field?.PropertyChanged += CommanderViewModel_PropertyChanged;
    }
  }

  [ObservableProperty] public partial bool IsBusy { get; set; }
  [ObservableProperty] public partial bool HasUnsavedChanges { get; set; }

  public string Name
  {
    get => Deck.Name;
    set => Deck.Name = value;
  }
  public int Size => Deck.DeckCards.Sum(x => x.Count) + (Deck.Commander != null ? 1 : 0) + (Deck.CommanderPartner != null ? 1 : 0);
  public double Price => Deck.DeckCards.Sum(x => x.Info.Price * x.Count) + (Deck.Commander?.Info.Price ?? 0) + (Deck.CommanderPartner?.Info.Price ?? 0);

  [NotNull] public IAsyncRelayCommand? NewDeckCommand => field ??= new NewDeck(this).Command;
  [NotNull] public IAsyncRelayCommand<string>? OpenDeckCommand => field ??= new OpenDeck(this).Command;
  [NotNull] public IAsyncRelayCommand? SaveDeckCommand => field ??= new SaveDeck(this).Command;
  [NotNull] public IAsyncRelayCommand? DeleteDeckCommand => field ??= new DeleteDeck(this).Command;
  [NotNull] public IAsyncRelayCommand? OpenEdhrecCommanderWebsiteCommand => field ??= new OpenEdhrecCommanderWebsite(this).Command;
  [NotNull] public IAsyncRelayCommand? ShowDeckTokensCommand => field ??= new ShowDeckTokens(this).Command;
  [NotNull] public IRelayCommand? OpenDeckTestingWindowCommand => field ??= new OpenDeckTestingWindow(this).Command;
  [NotNull] public IRelayCommand? OpenEdhrecSearchWindowCommand => field ??= new OpenEdhrecSearchWindow(this).Command;

  private void Deck_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
  {
    if (e.PropertyName == nameof(DeckEditorMTGDeck.Commander) && Deck.Commander != null)
      Deck.Commander.PropertyChanged -= DeckCard_PropertyChanged;
    else if (e.PropertyName == nameof(DeckEditorMTGDeck.CommanderPartner) && Deck.CommanderPartner != null)
      Deck.CommanderPartner.PropertyChanged -= DeckCard_PropertyChanged;
  }

  private void Deck_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName
      is (nameof(DeckEditorMTGDeck.Commander))
      or (nameof(DeckEditorMTGDeck.CommanderPartner)))
    {
      if (e.PropertyName == nameof(DeckEditorMTGDeck.Commander) && Deck.Commander != null)
      {
        Deck.Commander.PropertyChanged += DeckCard_PropertyChanged;
        Deck.Commander.ChangePrintCommand = new ChangeCardPrint(this).Command;
      }
      else if (e.PropertyName == nameof(DeckEditorMTGDeck.CommanderPartner) && Deck.CommanderPartner != null)
      {
        Deck.CommanderPartner.PropertyChanged += DeckCard_PropertyChanged;
        Deck.CommanderPartner.ChangePrintCommand = new ChangeCardPrint(this).Command;
      }

      OnPropertyChanged(nameof(Size));
      OnPropertyChanged(nameof(Price));

      ShowDeckTokensCommand.NotifyCanExecuteChanged();
      OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
      OpenEdhrecSearchWindowCommand.NotifyCanExecuteChanged();
    }
    else if (e.PropertyName == nameof(DeckEditorMTGDeck.Name))
      OnPropertyChanged(nameof(Name));
  }

  private void CardList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is IList newItems)
      AttachCommandsToCards(newItems.OfType<DeckEditorMTGCard>());
  }

  private void DeckCards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    CardList_CollectionChanged(sender, e);

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

  private void UndoStack_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    => HasUnsavedChanges = e.Action != NotifyCollectionChangedAction.Reset;

  private void AttachCommandsToCards(IEnumerable<DeckEditorMTGCard> cards)
  {
    foreach (var item in cards)
    {
      item.ChangePrintCommand = new ChangeCardPrint(this).Command;
      item.ChangeCountCommand = new ChangeCardCount(this).Command;
    }
  }
}