using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class CardListViewModel(ObservableCollection<DeckEditorMTGCard> cards, IMTGCardImporter importer) : ICardListViewModel
{
  public ObservableCollection<DeckEditorMTGCard> Cards { get; } = cards;
  public IMTGCardImporter Importer { get; } = importer;

  public ReversibleCommandStack UndoStack { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;
  public CardListConfirmers Confirmers { get; init; } = new();

  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? AddCardCommand => field ??= new AddCard(Cards, UndoStack, Confirmers).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? RemoveCardCommand => field ??= new RemoveCard(Cards, UndoStack).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? BeginMoveFromCommand => field ??= new MoveCard.BeginMoveFrom(Cards, UndoStack).Command;
  [NotNull] public IAsyncRelayCommand<DeckEditorMTGCard>? BeginMoveToCommand => field ??= new MoveCard.BeginMoveTo(Cards, UndoStack, Confirmers).Command;
  [NotNull] public IRelayCommand<DeckEditorMTGCard>? ExecuteMoveCommand => field ??= new MoveCard.ExecuteMove(UndoStack).Command;
  [NotNull] public IRelayCommand? ClearCommand => field ??= new Clear(Cards, UndoStack).Command;
  [NotNull] public IAsyncRelayCommand<string>? ImportCardsCommand => field ??= new ImportCards(Cards, UndoStack, Confirmers, Worker, Importer, Notifier).Command;
  [NotNull] public IAsyncRelayCommand<string>? ExportCardsCommand => field ??= new ExportCards(Cards, Confirmers, Notifier, ClipboardService).Command;
}