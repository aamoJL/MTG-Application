using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleAddCardsAction(IList<DeckEditorMTGCard> collection) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
{
  protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    if (cards.Any(x => collection.Any(y => y.Info.Name == x.Info.Name)))
      throw new InvalidOperationException("Cards are already in the collection");

    foreach (var card in cards)
      collection.Add(card);
  }

  protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    => new ReversibleRemoveCardsAction(collection).Action.Invoke(cards);
}