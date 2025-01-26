using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardPrintChangeAction : ReversibleAction<(DeckEditorMTGCard Card, MTGCardInfo Info)>
  {
    public ReversibleCardPrintChangeAction(IList<DeckEditorMTGCard> collection)
    {
      Collection = collection;
      Action = ActionMethod;
      ReverseAction = ReverseActionMethod;
    }

    public DeckEditorMTGCard? Card { get; set; }

    private IList<DeckEditorMTGCard> Collection { get; }

    protected void ActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
    {
      if ((Card ??= Collection.FirstOrDefault(x => x.Info.Name == param.Card.Info.Name)) is DeckEditorMTGCard card)
        CardPrintChange(card, param.Info);
    }

    protected void ReverseActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
      => ActionMethod(param);

    private void CardPrintChange(DeckEditorMTGCard card, MTGCardInfo info)
      => card.Info = info;
  }
}