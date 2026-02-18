using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleRemoveCardsAction(IList<DeckEditorMTGCard> collection) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
{
  private IList<DeckEditorMTGCard> Collection { get; } = collection;

  protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    foreach (var card in cards)
    {
      if (Collection.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        if (existingCard.Count <= card.Count)
          Collection.Remove(existingCard);
        else
          existingCard.Count -= card.Count;
      }
    }
  }

  protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    => new ReversibleAddCardsAction(Collection).Action(cards);
}