using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddGroupAction(IList<DeckEditorCardGroup> collection) : ReversibleAction<DeckEditorCardGroup>
  {
    protected override void ActionMethod(DeckEditorCardGroup group)
    {
      if (string.IsNullOrEmpty(group.GroupKey)) return;
      if (collection.Any(x => x.GroupKey == group.GroupKey)) return;

      // Find the alphabetical index of the key. Empty key will always be the last item
      if (collection.TryFindIndex(x => x.GroupKey == string.Empty || x.GroupKey.CompareTo(group) >= 0, out var i) && i >= 0)
        collection.Insert(i, group);
      else
        collection.Add(group);
    }

    protected override void ReverseActionMethod(DeckEditorCardGroup group)
       => new ReversibleRemoveGroupAction(collection).Action.Invoke(group);
  }
}