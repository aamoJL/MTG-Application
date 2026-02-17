using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddCardsToGroupAction(DeckEditorCardGroup group) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
  {
    private string? _oldGroup = null;

    protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    {
      _oldGroup ??= cards.FirstOrDefault()?.Group;

      foreach (var card in cards)
      {
        card.Group = group.GroupKey;

        if (group.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
          existingCard.Count += card.Count;
        else
          group.Add(card);
      }
    }

    protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    {
      new ReversibleRemoveCardsFromGroupAction(group).Action(cards);

      if (_oldGroup != null)
      {
        foreach (var card in cards)
          card.Group = _oldGroup;
      }
    }
  }
}