using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleAddCardsToGroupAction(DeckEditorCardGroup group) : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
{
  private readonly List<string> _oldGroups = [];

  protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    if (!cards.Any()) return;

    _oldGroups.Clear();

    if (cards.Any(group.SourceContains))
      throw new InvalidOperationException("Card is already in the source");

    foreach (var card in cards)
    {
      _oldGroups.Add(card.Group);
      group.AddToSource(card);
      card.Group = group.GroupKey;
    }
  }

  protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
  {
    if (!cards.Any()) return;

    var i = 0;

    foreach (var card in cards)
    {
      group.RemoveFromSource(card);
      card.Group = _oldGroups[i];
      i++;
    }
  }
}