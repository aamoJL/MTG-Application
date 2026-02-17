using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddCardsAction(IList<DeckEditorMTGCard> collection) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
  {
    protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    {
      foreach (var card in cards)
      {
        if (collection.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
          existingCard.Count += card.Count;
        else
          collection.Add(card);
      }
    }

    protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => new ReversibleRemoveCardsAction(collection).Action.Invoke(cards);
  }
}