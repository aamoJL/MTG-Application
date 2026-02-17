using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardGroupChangeAction : ReversibleAction<(DeckEditorMTGCard Card, string Group)>
  {
    protected override void ActionMethod((DeckEditorMTGCard Card, string Group) param)
    {
      if (param.Card == null || param.Card.Group == param.Group)
        return;

      param.Card.Group = param.Group;
    }

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, string Group) param)
      => ActionMethod(param);
  }
}