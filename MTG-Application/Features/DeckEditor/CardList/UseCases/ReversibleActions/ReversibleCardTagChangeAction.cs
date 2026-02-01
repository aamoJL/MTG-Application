using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardTagChangeAction : ReversibleAction<(DeckEditorMTGCard Card, CardTag? Value)>
  {
    public ReversibleCardTagChangeAction()
    {
      Action = ActionMethod;
      ReverseAction = ReverseActionMethod;
    }

    protected void ActionMethod((DeckEditorMTGCard Card, CardTag? Value) param)
      => ChangeTag(param.Card, param.Value);

    protected void ReverseActionMethod((DeckEditorMTGCard Card, CardTag? Value) param)
      => ActionMethod(param);

    private void ChangeTag(DeckEditorMTGCard card, CardTag? value)
      => card.CardTag = value;
  }
}