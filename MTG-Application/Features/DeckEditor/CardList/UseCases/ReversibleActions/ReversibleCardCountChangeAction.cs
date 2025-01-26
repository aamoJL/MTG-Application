using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardCountChangeAction : ReversibleAction<(DeckEditorMTGCard Card, int Value)>
  {
    public ReversibleCardCountChangeAction()
    {
      Action = ActionMethod;
      ReverseAction = ReverseActionMethod;
    }

    protected void ActionMethod((DeckEditorMTGCard Card, int Value) param)
      => CountChange(param.Card, param.Value);

    protected void ReverseActionMethod((DeckEditorMTGCard Card, int Value) param)
      => ActionMethod(param);

    private void CountChange(DeckEditorMTGCard card, int value)
      => card.Count = value;
  }
}