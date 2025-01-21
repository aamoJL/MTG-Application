using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services.Factories;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static MTGApplication.Features.DeckEditor.Editor.UseCases.DeckEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;
public partial class DeckEditorViewModel(
  IMTGCardImporter importer,
  Notifier? notifier = null,
  DeckEditorConfirmers? confirmers = null) : ObservableObject, ISavable, IWorker
{
  public DeckEditorMTGDeck Deck
  {
    get;
    set
    {
      if (!SetProperty(ref field, value))
        return;

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
  } = new();

  public IMTGCardImporter Importer { get; } = importer;
  public DeckEditorConfirmers Confirmers { get; } = confirmers ?? new();
  public Notifier Notifier { get; } = notifier ?? new();
  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public ReversibleCommandStack UndoStack { get; } = new();

  [NotNull] public GroupedCardListViewModel? DeckCardList => field ??= new DeckEditorCardListFactory(this).CreateGroupedCardListViewModel(Deck.DeckCards, OnCardListChanged);
  [NotNull] public CardListViewModel? MaybeCardList => field ??= new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Maybelist, OnCardListChanged);
  [NotNull] public CardListViewModel? WishCardList => field ??= new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Wishlist, OnCardListChanged);
  [NotNull] public CardListViewModel? RemoveCardList => field ??= new DeckEditorCardListFactory(this).CreateCardListViewModel(Deck.Removelist, OnCardListChanged);
  [NotNull] public CommanderViewModel? Commander => field ??= new DeckEditorCommanderFactory(this).CreateCommanderViewModel(Deck.Commander, onChange: (card) => { Deck.Commander = card; OnCommandersChanged(); });
  [NotNull] public CommanderViewModel? Partner => field ??= new DeckEditorCommanderFactory(this).CreateCommanderViewModel(Deck.CommanderPartner, onChange: (card) => { Deck.CommanderPartner = card; OnCommandersChanged(); });

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
  [NotNull] public IAsyncRelayCommand? SaveDeckCommand => new SaveDeck(this).Command;
  [NotNull] public IAsyncRelayCommand? DeleteDeckCommand => field ??= new DeleteDeck(this).Command;
  [NotNull] public IRelayCommand? UndoCommand => new Undo(this).Command;
  [NotNull] public IRelayCommand? RedoCommand => field ??= new Redo(this).Command;
  [NotNull] public IAsyncRelayCommand? OpenEdhrecCommanderWebsiteCommand => field ??= new OpenEdhrecCommanderWebsite(this).Command;
  [NotNull] public IAsyncRelayCommand? ShowDeckTokensCommand => new ShowDeckTokens(this).Command;
  [NotNull] public IRelayCommand? OpenDeckTestingWindowCommand => field ??= new OpenDeckTestingWindow(this).Command;
  [NotNull] public IRelayCommand? OpenEdhrecSearchWindowCommand => field ??= new OpenEdhrecSearchWindow(this).Command;

  private void OnCardListChanged()
  {
    HasUnsavedChanges = true;

    OnPropertyChanged(nameof(Size));
    OnPropertyChanged(nameof(Price));
    ShowDeckTokensCommand?.NotifyCanExecuteChanged();
    OpenDeckTestingWindowCommand?.NotifyCanExecuteChanged();
  }

  private void OnCommandersChanged()
  {
    HasUnsavedChanges = true;

    OnPropertyChanged(nameof(Size));
    OnPropertyChanged(nameof(Price));
    OpenEdhrecCommanderWebsiteCommand!.NotifyCanExecuteChanged();
    OpenEdhrecSearchWindowCommand!.NotifyCanExecuteChanged();
  }
}