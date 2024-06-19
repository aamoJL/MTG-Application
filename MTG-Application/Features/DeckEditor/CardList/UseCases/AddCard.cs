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

public partial class CardListViewModelCommands
{
  public class AddCard(CardListViewModel viewmodel) : ViewModelAsyncCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override async Task Execute(DeckEditorMTGCard card)
    {
      if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) != null)
      {
        if (await Viewmodel.Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
          Viewmodel.UndoStack.PushAndExecute(new ReversibleCollectionCommand<DeckEditorMTGCard>(card, Viewmodel.CardCopier) { ReversibleAction = new ReversibleAddCardAction(Viewmodel) });
      }
      else
        Viewmodel.UndoStack.PushAndExecute(new ReversibleCollectionCommand<DeckEditorMTGCard>(card, Viewmodel.CardCopier) { ReversibleAction = new ReversibleAddCardAction(Viewmodel) });
    }
  }
}