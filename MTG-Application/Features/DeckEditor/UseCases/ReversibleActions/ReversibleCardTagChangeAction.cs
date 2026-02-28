using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleCardTagChangeAction : ReversibleAction<(DeckEditorMTGCard Card, CardTag? Value)>
{
  protected override void ActionMethod((DeckEditorMTGCard Card, CardTag? Value) param)
    => param.Card.CardTag = param.Value;

  protected override void ReverseActionMethod((DeckEditorMTGCard Card, CardTag? Value) param)
    => ActionMethod(param);
}