using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleRemoveCardsAction(IList<DeckEditorMTGCard> collection) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
{
  protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    if (cards.Any(x => !collection.Contains(x)))
      throw new InvalidOperationException("Cards are not in the collection");

    foreach (var card in cards)
      collection.Remove(card);
  }

  protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    => new ReversibleAddCardsAction(collection).Action(cards);
}