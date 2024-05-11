﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.DeckEditor;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  private MTGCardDeck deck = new();

  public DeckEditorViewModel()
  {
    DeckCards = new(CardAPI) { OnChange = OnDeckCardsChanged, UndoStack = UndoStack, Worker = this };
    MaybeCards = new(CardAPI) { OnChange = OnDeckCardsChanged, UndoStack = UndoStack, Worker = this };
  }

  public DeckEditorViewModel(MTGCardDeck deck) : this() => Deck = deck;

  private MTGCardDeck Deck
  {
    get => deck;
    set
    {
      deck = value;

      DeckCards.Cards = deck.DeckCards;
      MaybeCards.Cards = deck.Maybelist;
      UndoStack.Clear();
      HasUnsavedChanges = false;

      OnPropertyChanged(nameof(DeckSize));
      OnPropertyChanged(nameof(DeckName));
      OnPropertyChanged(nameof(Commander));
      OnPropertyChanged(nameof(Partner));
      SaveDeckCommand.NotifyCanExecuteChanged();
      DeleteDeckCommand.NotifyCanExecuteChanged();
    }
  }
  private ReversibleCommandStack UndoStack { get; } = new();

  public ICardAPI<MTGCard> CardAPI { private get; init; } = App.MTGCardAPI;
  public IRepository<MTGCardDeckDTO> Repository { private get; init; } = new DeckDTORepository();

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public DeckEditorConfirmers Confirmers { get; init; } = new();
  public DeckEditorNotifier Notifier { get; init; } = new();
  public CardFilters CardFilters { get; init; } = new();
  public CardSorter CardSorter { get; init; } = new();

  public CardListViewModel DeckCards { get; }
  public CardListViewModel MaybeCards { get; }

  public MTGCard Commander
  {
    get => Deck.Commander;
    set
    {
      Deck.Commander = value;
      OnPropertyChanged(nameof(Commander));
    }
  }
  public MTGCard Partner
  {
    get => Deck.CommanderPartner;
    set
    {
      Deck.CommanderPartner = value;
      OnPropertyChanged(nameof(Partner));
    }
  }
  public string DeckName
  {
    get => Deck.Name;
    set
    {
      Deck.Name = value;
      OnPropertyChanged(nameof(DeckName));
    }
  }
  public int DeckSize => Deck.DeckSize;

  public async Task<bool> ConfirmUnsavedChanges()
  {
    if (!HasUnsavedChanges) return true;

    var result = await new SaveUnsavedChanges(Repository)
    {
      UnsavedChangesConfirmation = Confirmers.SaveUnsavedChanges,
      SaveConfirmation = Confirmers.SaveDeck,
      OverrideConfirmation = Confirmers.OverrideDeck,
      Worker = this
    }.Execute(Deck);

    switch (result)
    {
      case ConfirmationResult.Yes: SendNotification(Notifier.Notifications.SaveSuccessNotification); return true;
      case ConfirmationResult.No: return true;
      case ConfirmationResult.Failure: SendNotification(Notifier.Notifications.SaveErrorNotification); return false;
      default: return false;
    }
  }

  private void SendNotification(NotificationService.Notification notification)
    => Notifier.Notify(notification);

  private void OnDeckCardsChanged()
  {
    HasUnsavedChanges = true;
    OnPropertyChanged(nameof(DeckSize));
  }
}

public partial class DeckEditorViewModel
{
  [RelayCommand]
  private async Task NewDeck()
  {
    if (await ConfirmUnsavedChanges())
      Deck = new();
  }

  [RelayCommand(CanExecute = nameof(CanExecuteOpenDeckCommand))]
  private async Task OpenDeck(string loadName = null)
  {
    if (!OpenDeckCommand.CanExecute(loadName)) return;

    if (await ConfirmUnsavedChanges())
    {
      var loadResult = await new LoadDeck(Repository, CardAPI)
      {
        LoadConfirmation = Confirmers.LoadDeck,
        Worker = this
      }.Execute(loadName);

      switch (loadResult.ConfirmResult)
      {
        case ConfirmationResult.Yes:
          Deck = loadResult.Deck;
          SendNotification(Notifier.Notifications.LoadSuccessNotification); break;
        case ConfirmationResult.Failure: SendNotification(Notifier.Notifications.LoadErrorNotification); break;
        default: break;
      }
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckCommand))]
  private async Task SaveDeck()
  {
    if (!SaveDeckCommand.CanExecute(null)) return;

    var result = await new SaveDeck(Repository)
    {
      SaveConfirmation = Confirmers.SaveDeck,
      OverrideConfirmation = Confirmers.OverrideDeck,
      Worker = this
    }.Execute(Deck);

    switch (result)
    {
      case ConfirmationResult.Yes:
        SendNotification(Notifier.Notifications.SaveSuccessNotification);
        OnPropertyChanged(nameof(DeckName));
        HasUnsavedChanges = false;
        break;
      case ConfirmationResult.Failure: SendNotification(Notifier.Notifications.SaveErrorNotification); return;
      default: break;
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckCommand))]
  private async Task DeleteDeck()
  {
    if (!DeleteDeckCommand.CanExecute(null)) return;

    var result = await new DeleteDeck(Repository)
    {
      DeleteConfirmation = Confirmers.DeleteDeck,
      Worker = this
    }.Execute(Deck);

    switch (result)
    {
      case ConfirmationResult.Yes:
        Deck = new();
        SendNotification(Notifier.Notifications.DeleteSuccessNotification); break;
      case ConfirmationResult.Failure: SendNotification(Notifier.Notifications.DeleteErrorNotification); break;
      default: return;
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteUndoCommand))] private void Undo() => UndoStack.Undo();

  [RelayCommand(CanExecute = nameof(CanExecuteRedoCommand))] private void Redo() => UndoStack.Redo();

  private bool CanExecuteSaveDeckCommand() => Deck.DeckCards.Count > 0;

  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);

  private bool CanExecuteOpenDeckCommand(string name) => name != string.Empty;

  private bool CanExecuteUndoCommand() => UndoStack.CanUndo;

  private bool CanExecuteRedoCommand() => UndoStack.CanRedo;
}