using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using static MTGApplication.Features.DeckEditor.CardListViewModelCommands;
using static MTGApplication.Features.DeckEditor.CardListViewModelCommands.ChangeCardCount;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModel(MTGCardImporter importer) : ViewModelBase
{
  [ObservableProperty] private ObservableCollection<DeckEditorMTGCard> cards = [];

  public MTGCardImporter Importer { get; } = importer;
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public CardListConfirmers Confirmers { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;
  public DeckEditorMTGCardCopier CardCopier { get; } = new();
  public CardFilters CardFilters { get; init; } = new();
  public CardSorter CardSorter { get; init; } = new();

  public Action OnChange { get; init; }

  public IAsyncRelayCommand<DeckEditorMTGCard> AddCardCommand => (addCard ??= new AddCard(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> RemoveCardCommand => (removeCard ??= new RemoveCard(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand => (beginMoveFrom ??= new MoveCard.BeginMoveFrom(this)).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand => (beginMoveTo ??= new MoveCard.BeginMoveTo(this)).Command;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand => (executeMove ??= new MoveCard.ExecuteMove(this)).Command;
  public IRelayCommand ClearCommand => (clear ??= new Clear(this)).Command;
  public IAsyncRelayCommand<string> ImportCardsCommand => (importCards ??= new ImportCards(this)).Command;
  public IAsyncRelayCommand<string> ExportCardsCommand => (exportCards ??= new ExportCards(this)).Command;
  public IRelayCommand<CardCountChangeArgs> ChangeCardCountCommand => (changeCardCount ??= new ChangeCardCount(this)).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> ChangeCardPrintCommand => (changeCardPrint ??= new ChangeCardPrint(this)).Command;

  private AddCard addCard;
  private RemoveCard removeCard;
  private MoveCard.BeginMoveFrom beginMoveFrom;
  private MoveCard.BeginMoveTo beginMoveTo;
  private MoveCard.ExecuteMove executeMove;
  private Clear clear;
  private ImportCards importCards;
  private ExportCards exportCards;
  private ChangeCardCount changeCardCount;
  private ChangeCardPrint changeCardPrint;
}