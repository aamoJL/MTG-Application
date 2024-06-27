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
  public class MoveCard
  {
    public class BeginMoveTo(CardListViewModel viewmodel) : ViewModelAsyncCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override async Task Execute(DeckEditorMTGCard card)
      {
        if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) != null)
        {
          if (await Viewmodel.Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
            Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(new ReversibleCollectionCommand<DeckEditorMTGCard>(card, Viewmodel.CardCopier)
            {
              ReversibleAction = new ReversibleAddCardAction(Viewmodel)
            });
          else
            Viewmodel.UndoStack.ActiveCombinedCommand.Cancel();
        }
        else
          Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(new ReversibleCollectionCommand<DeckEditorMTGCard>(card, Viewmodel.CardCopier)
          {
            ReversibleAction = new ReversibleAddCardAction(Viewmodel)
          });
      }
    }

    public class BeginMoveFrom(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard card)
        => Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(
          new ReversibleCollectionCommand<DeckEditorMTGCard>(card, Viewmodel.CardCopier)
          {
            ReversibleAction = new ReversibleRemoveCardAction(Viewmodel)
          });
    }

    public class ExecuteMove(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard card)
        => Viewmodel.UndoStack.PushAndExecuteActiveCombinedCommand();
    }
  }
}