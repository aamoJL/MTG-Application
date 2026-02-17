using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardPrintChangeAction : ReversibleAction<(DeckEditorMTGCard Card, MTGCardInfo Info)>
  {
    protected override void ActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
      => param.Card.Info = param.Info;

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
      => ActionMethod(param);
  }
}