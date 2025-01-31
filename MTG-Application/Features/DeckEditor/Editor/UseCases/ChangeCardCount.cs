using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public record CardCountChangeArgs(DeckEditorMTGCard Card, int Value);

  public class ChangeCardCount(DeckEditorViewModel viewmodel) : ViewModelCommand<DeckEditorViewModel, CardCountChangeArgs>(viewmodel)
  {
    protected override bool CanExecute(CardCountChangeArgs? args)
      => args != null && args.Card.Count != args.Value && args.Value > 0;

    protected override void Execute(CardCountChangeArgs? args)
    {
      if (!CanExecute(args))
        return;

      var (card, newValue) = args!;

      Viewmodel.UndoStack.PushAndExecute(
        new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(card, card.Count, newValue)
        {
          ReversibleAction = new ReversibleCardCountChangeAction()
        });
    }
  }
}