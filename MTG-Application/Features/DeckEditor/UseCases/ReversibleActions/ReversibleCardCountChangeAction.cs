using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleCardCountChangeAction : ReversibleAction<(DeckEditorMTGCard Card, int Value)>
{
  protected override void ActionMethod((DeckEditorMTGCard Card, int Value) param)
    => param.Card.Count = param.Value;

  protected override void ReverseActionMethod((DeckEditorMTGCard Card, int Value) param)
    => ActionMethod(param);
}