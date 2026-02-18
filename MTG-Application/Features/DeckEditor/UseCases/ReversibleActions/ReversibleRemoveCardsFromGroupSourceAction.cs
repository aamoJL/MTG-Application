using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleRemoveCardsFromGroupSourceAction(DeckEditorCardGroup group) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
{
  protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    foreach (var card in cards)
    {
      if (group.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        existingCard.Group = string.Empty;
        group.RemoveFromSource(existingCard);
      }
    }
  }

  protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    => new ReversibleAddCardsToGroupAction(group).Action(cards);
}