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

  public Action OnChange { get; init; }

  public IAsyncRelayCommand<DeckEditorMTGCard> AddCardCommand => new AddCard(this).Command;
  public IRelayCommand<DeckEditorMTGCard> RemoveCardCommand => new RemoveCard(this).Command;
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand => new MoveCard.BeginMoveFrom(this).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand => new MoveCard.BeginMoveTo(this).Command;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand => new MoveCard.ExecuteMove(this).Command;
  public IRelayCommand ClearCommand => new Clear(this).Command;
  public IAsyncRelayCommand<string> ImportCardsCommand => new ImportCards(this).Command;
  public IAsyncRelayCommand<string> ExportCardsCommand => new ExportCards(this).Command;
  public IRelayCommand<CardCountChangeArgs> ChangeCardCountCommand => new ChangeCardCount(this).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> ChangeCardPrintCommand => new ChangeCardPrint(this).Command;
}