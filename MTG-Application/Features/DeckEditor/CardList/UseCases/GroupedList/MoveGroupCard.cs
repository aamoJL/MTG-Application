using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardGroupViewModelCommands
{
  public class MoveGroupCard
  {
    public class BeginMoveFrom(CardGroupViewModel viewmodel) : SyncCommand<DeckEditorMTGCard>
    {
      public CardGroupViewModel Viewmodel { get; } = viewmodel;

      protected override bool CanExecute(DeckEditorMTGCard? card) => card != null;

      protected override void Execute(DeckEditorMTGCard? card)
      {
        if (!CanExecute(card))
          return;

        Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(new CombinedReversibleCommand()
        {
          Commands = [
            new ReversibleCollectionCommand<DeckEditorMTGCard>(card!)
            {
              ReversibleAction = new ReversibleRemoveCardsAction(Viewmodel.Source)
            },
            new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(card!, card!.Group, card.Group)
            {
              ReversibleAction = new ReversibleCardGroupChangeAction()
            }
          ]
        });
      }
    }

    public class BeginMoveTo(CardGroupViewModel viewmodel) : AsyncCommand<DeckEditorMTGCard>
    {
      public CardGroupViewModel Viewmodel { get; } = viewmodel;

      protected override async Task Execute(DeckEditorMTGCard? card)
      {
        if (card == null)
          return;

        var combinedCommands = Viewmodel.UndoStack.ActiveCombinedCommand.Commands;

        if (Viewmodel.Source.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
        {
          if (existingCard == card)
          {
            // Card is from the same list
            // Change move action to group change action
            Viewmodel.UndoStack.ActiveCombinedCommand.Cancel();

            Viewmodel.UndoStack.PushAndExecute(new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(existingCard, existingCard.Group, Viewmodel.Key)
            {
              ReversibleAction = new ReversibleCardGroupChangeAction()
            });
          }
          else if (await Viewmodel.Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
          {
            combinedCommands.Add(new CombinedReversibleCommand()
            {
              Commands = [
                new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(existingCard, existingCard.Count, card.Count + existingCard.Count)
                {
                  ReversibleAction = new ReversibleCardCountChangeAction()
                },
                new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(existingCard, existingCard.Group, Viewmodel.Key)
                {
                  ReversibleAction = new ReversibleCardGroupChangeAction()
                }]
            });
          }
          else
            Viewmodel.UndoStack.ActiveCombinedCommand.Cancel();
        }
        else
        {
          // Card not in the list, add
          combinedCommands.Add(new CombinedReversibleCommand()
          {
            Commands = [
              new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
              {
                ReversibleAction = new ReversibleAddCardsAction(Viewmodel.Source)
              },
              new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(card, card.Group, Viewmodel.Key)
              {
                ReversibleAction = new ReversibleCardGroupChangeAction()
              }]
          });
        }
      }
    }

    public class ExecuteMove(CardGroupViewModel viewmodel) : SyncCommand<DeckEditorMTGCard>
    {
      public CardGroupViewModel Viewmodel { get; } = viewmodel;

      protected override void Execute(DeckEditorMTGCard? card)
        => Viewmodel.UndoStack.PushAndExecuteActiveCombinedCommand();
    }
  }
}