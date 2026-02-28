using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleRemoveGroupAction(IList<DeckEditorCardGroup> collection) : ReversibleAction<DeckEditorCardGroup>
{
  private DeckEditorMTGCard[] AffectedCards { get; set; } = [];

  protected override void ActionMethod(DeckEditorCardGroup group)
  {
    if (group == null) return;

    AffectedCards = [.. group.Cards];

    // Move cards to default group
    foreach (var card in AffectedCards)
      card.Group = string.Empty;

    if (collection.TryFindIndex(x => x.GroupKey == group.GroupKey, out var index))
      collection.RemoveAt(index);
  }

  protected override void ReverseActionMethod(DeckEditorCardGroup group)
  {
    new ReversibleAddGroupAction(collection).Action(group);

    // Move old cards back
    foreach (var card in AffectedCards)
      card.Group = group.GroupKey;
  }
}