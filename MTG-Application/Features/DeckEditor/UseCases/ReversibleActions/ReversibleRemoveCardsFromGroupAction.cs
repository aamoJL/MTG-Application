using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRemoveCardsFromGroupAction(DeckEditorCardGroup group) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
  {
    protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    {
      foreach (var card in cards)
      {
        if (group.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
        {
          if (existingCard.Count <= card.Count)
            group.Remove(existingCard);
          else
            existingCard.Count -= card.Count;
        }
      }
    }

    protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => new ReversibleAddCardsToGroupAction(group).Action(cards);
  }
}