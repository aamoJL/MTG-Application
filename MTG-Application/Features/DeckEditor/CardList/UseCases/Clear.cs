using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelCommands
{
  public class Clear(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Cards.Any();

    protected override void Execute()
    {
      if (!CanExecute()) return;

      Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(Viewmodel.Cards, Viewmodel.CardCopier)
        {
          ReversibleAction = new CardListViewModelReversibleActions.ReversibleRemoveCardAction(Viewmodel)
        });
    }
  }
}