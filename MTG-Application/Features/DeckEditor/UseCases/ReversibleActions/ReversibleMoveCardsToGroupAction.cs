using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleMoveCardsToGroupAction(DeckEditorCardGroup group) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
{
  private readonly List<string> _oldGroups = [];

  protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    if (!cards.Any()) return;

    _oldGroups.Clear();

    foreach (var card in cards)
    {
      if (group.GetFromSource(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard sourceCard)
      {
        _oldGroups.Add(card.Group);
        card.Group = group.GroupKey;
      }
      else
        throw new InvalidOperationException("Card is not in the same source");
    }
  }

  protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    if (!cards.Any()) return;

    var i = 0;

    foreach (var card in cards)
    {
      card.Group = _oldGroups[i];
      i++;
    }
  }
}