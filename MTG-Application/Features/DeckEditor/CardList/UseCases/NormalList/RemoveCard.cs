using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public class RemoveCard(IList<DeckEditorMTGCard> cards, ReversibleCommandStack undoStack) : SyncCommand<DeckEditorMTGCard>
  {
    private IList<DeckEditorMTGCard> Cards { get; } = cards;
    private ReversibleCommandStack UndoStack { get; } = undoStack;

    protected override void Execute(DeckEditorMTGCard? card)
    {
      if (card == null)
        return;

      UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
        {
          ReversibleAction = new ReversibleRemoveCardsAction(Cards)
        });
    }
  }
}