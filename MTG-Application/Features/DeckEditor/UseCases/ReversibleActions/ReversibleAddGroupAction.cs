using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.ReversibleCommandService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleAddGroupAction(IList<DeckEditorCardGroup> collection) : ReversibleAction<DeckEditorCardGroup>
{
  protected override void ActionMethod(DeckEditorCardGroup group)
  {
    if (string.IsNullOrEmpty(group.GroupKey)) return;
    if (collection.Any(x => x.GroupKey == group.GroupKey)) return;

    // Find the alphabetical index of the key. Empty key will always be the last item
    if (collection.TryFindIndex(x => x.GroupKey == string.Empty || x.GroupKey.CompareTo(group.GroupKey) >= 0, out var i) && i >= 0)
      collection.Insert(i, group);
    else
      throw new Exception("No index was found to add the group");
  }

  protected override void ReverseActionMethod(DeckEditorCardGroup group)
      => new ReversibleRemoveGroupAction(collection).Action.Invoke(group);
}