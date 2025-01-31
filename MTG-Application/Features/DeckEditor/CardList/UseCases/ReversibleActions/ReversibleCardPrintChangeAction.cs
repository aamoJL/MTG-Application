using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardPrintChangeAction : ReversibleAction<(DeckEditorMTGCard Card, MTGCardInfo Info)>
  {
    public ReversibleCardPrintChangeAction()
    {
      Action = ActionMethod;
      ReverseAction = ReverseActionMethod;
    }

    protected void ActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
      => CardPrintChange(param.Card, param.Info);

    protected void ReverseActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
      => CardPrintChange(param.Card, param.Info);

    private void CardPrintChange(DeckEditorMTGCard card, MTGCardInfo info)
      => card.Info = info;
  }
}