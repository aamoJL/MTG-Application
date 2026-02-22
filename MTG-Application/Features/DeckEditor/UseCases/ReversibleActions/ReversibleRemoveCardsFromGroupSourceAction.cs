using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.ReversibleCommandService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleRemoveCardsFromGroupSourceAction(DeckEditorCardGroup group) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
{
  protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    var indices = group.Cards.FindItemIndices([.. cards], (x, y) => x.Info == y.Info);

    if (indices.Any(x => x == -1))
      throw new InvalidOperationException("Cards are not in the collection");

    foreach (var i in indices)
    {
      var card = group.Cards[i];
      card.Group = string.Empty;
      group.RemoveFromSource(card);
    }
  }

  protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    => new ReversibleAddCardsToGroupAction(group).Action(cards);
}