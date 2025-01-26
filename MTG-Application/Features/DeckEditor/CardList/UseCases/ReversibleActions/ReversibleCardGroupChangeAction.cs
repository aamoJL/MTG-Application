using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardGroupChangeAction : ReversibleAction<(DeckEditorMTGCard Card, string Group)>
  {
    public ReversibleCardGroupChangeAction()
    {
      Action = ActionMethod;
      ReverseAction = ReverseActionMethod;
    }

    protected void ActionMethod((DeckEditorMTGCard Card, string Group) param)
      => GroupChange(param.Card, param.Group);

    protected void ReverseActionMethod((DeckEditorMTGCard Card, string Group) param)
      => ActionMethod(param);

    private void GroupChange(DeckEditorMTGCard card, string key)
    {
      if (card == null || card.Group == key)
        return;

      card.Group = key;
    }
  }
}