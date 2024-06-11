using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelCommands
{
  public class ChangeCardCount(CardListViewModel viewmodel) : ViewModelCommand<CardListViewModel, ChangeCardCount.CardCountChangeArgs>(viewmodel)
  {
    public record CardCountChangeArgs(DeckEditorMTGCard Card, int Value);

    protected override bool CanExecute(CardCountChangeArgs args) => args.Card.Count != args.Value && args.Value > 0;

    protected override void Execute(CardCountChangeArgs args)
    {
      if (!CanExecute(args)) return;

      var (card, newValue) = args;

      Viewmodel.UndoStack.PushAndExecute(
        new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(card, card.Count, newValue, Viewmodel.CardCopier)
        {
          ReversibleAction = new CardListViewModelReversibleActions.ReversibleCardCountChangeAction(Viewmodel)
        });
    }
  }
}