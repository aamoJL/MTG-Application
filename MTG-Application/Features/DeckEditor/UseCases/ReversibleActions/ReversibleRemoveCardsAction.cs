using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.ReversibleCommandService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleRemoveCardsAction(IList<DeckEditorMTGCard> collection) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
{
  private readonly List<DeckEditorMTGCard> _removedCards = [];

  protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    var indices = collection.FindItemIndices([.. cards], (x, y) => x.Info == y.Info);

    if (indices.Any(x => x == -1))
      throw new InvalidOperationException("Cards are not in the collection");

    _removedCards.Clear();
    _removedCards.AddRange(indices.Select(i => collection[i]));

    foreach (var item in _removedCards)
      collection.Remove(item);
  }

  protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    => new ReversibleAddCardsAction(collection).Action(_removedCards);
}