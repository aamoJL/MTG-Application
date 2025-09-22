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
  public class AddCard(IList<DeckEditorMTGCard> cards, ReversibleCommandStack undoStack, CardListConfirmers confirmers) : AsyncCommand<DeckEditorMTGCard>
  {
    private IList<DeckEditorMTGCard> Cards { get; } = cards;
    private ReversibleCommandStack UndoStack { get; } = undoStack;
    private CardListConfirmers Confirmers { get; } = confirmers;

    protected override async Task Execute(DeckEditorMTGCard? card)
    {
      if (card == null)
        return;

      if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) == null
        || await Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
      {
        UndoStack.PushAndExecute(
          new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
          {
            ReversibleAction = new ReversibleAddCardsAction(Cards)
          });
      }
    }
  }
}