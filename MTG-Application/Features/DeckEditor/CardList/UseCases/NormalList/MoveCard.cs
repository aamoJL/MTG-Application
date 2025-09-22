using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public class MoveCard
  {
    public class BeginMoveTo(IList<DeckEditorMTGCard> cards, ReversibleCommandStack undoStack, CardListConfirmers confirmers) : AsyncCommand<DeckEditorMTGCard>
    {
      private IList<DeckEditorMTGCard> Cards { get; } = cards;
      private ReversibleCommandStack UndoStack { get; } = undoStack;
      private CardListConfirmers Confirmers { get; } = confirmers;

      protected override async Task Execute(DeckEditorMTGCard? card)
      {
        if (card == null)
          return;

        if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
        {
          // Card already in the list
          if (await Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
          {
            UndoStack.ActiveCombinedCommand.Commands.Add(
              new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(existingCard, existingCard.Count, card.Count + existingCard.Count)
              {
                ReversibleAction = new ReversibleCardCountChangeAction()
              });
          }
          else
            UndoStack.ActiveCombinedCommand.Cancel();
        }
        else
          UndoStack.ActiveCombinedCommand.Commands.Add(
            new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
            {
              ReversibleAction = new ReversibleAddCardsAction(Cards)
            });
      }
    }

    public class BeginMoveFrom(IList<DeckEditorMTGCard> cards, ReversibleCommandStack undoStack) : SyncCommand<DeckEditorMTGCard>
    {
      private IList<DeckEditorMTGCard> Cards { get; } = cards;
      private ReversibleCommandStack UndoStack { get; } = undoStack;

      protected override void Execute(DeckEditorMTGCard? card)
      {
        if (card == null)
          return;

        UndoStack.ActiveCombinedCommand.Commands.Add(
          new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
          {
            ReversibleAction = new ReversibleRemoveCardsAction(Cards)
          });
      }
    }

    public class ExecuteMove(ReversibleCommandStack undoStack) : SyncCommand<DeckEditorMTGCard>
    {
      private ReversibleCommandStack UndoStack { get; } = undoStack;

      protected override void Execute(DeckEditorMTGCard? _)
        => UndoStack.PushAndExecuteActiveCombinedCommand();
    }
  }
}