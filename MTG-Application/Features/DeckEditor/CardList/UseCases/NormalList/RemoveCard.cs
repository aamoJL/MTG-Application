using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public class RemoveCard(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override void Execute(DeckEditorMTGCard? card)
    {
      if (card == null)
        return;

      Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
        {
          ReversibleAction = new ReversibleRemoveCardsAction(Viewmodel.Cards)
        });
    }
  }
}