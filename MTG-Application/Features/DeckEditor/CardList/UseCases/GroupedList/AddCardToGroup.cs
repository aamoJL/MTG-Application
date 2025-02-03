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
  public class AddCardToGroup(CardGroupViewModel viewmodel) : ViewModelAsyncCommand<CardGroupViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override async Task Execute(DeckEditorMTGCard? card)
    {
      if (card == null)
        return;

      if (Viewmodel.Source.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        // Confirm change on existing card
        if (await Viewmodel.Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
        {
          Viewmodel.UndoStack.PushAndExecute(new CombinedReversibleCommand()
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
      }
      else
      {
        // Add nonexistent card
        Viewmodel.UndoStack.PushAndExecute(new CombinedReversibleCommand()
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
}