using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelCommands
{
  public class RemoveCard(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override void Execute(DeckEditorMTGCard card)
      => Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(card, Viewmodel.CardCopier)
        {
          ReversibleAction = new CardListViewModelReversibleActions.ReversibleRemoveCardAction(Viewmodel)
        });
  }
}