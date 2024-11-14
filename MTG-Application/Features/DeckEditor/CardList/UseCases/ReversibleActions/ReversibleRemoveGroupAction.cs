using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRemoveGroupAction(GroupedCardListViewModel viewmodel) : ViewModelReversibleAction<GroupedCardListViewModel, string>(viewmodel)
  {
    private DeckEditorMTGCard[] AffectedCards { get; set; } = [];

    protected override void ActionMethod(string key)
    {
      AffectedCards = [];

      if (Viewmodel.Groups.FirstOrDefault(x => x.Key == key) is not CardGroupViewModel group)
        return;

      if (Viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty) is CardGroupViewModel defaultGroup)
      {
        AffectedCards = [.. group.Items];

        foreach (var card in AffectedCards)
          new ReversibleCardGroupChangeAction(Viewmodel).Action.Invoke((card, defaultGroup.Key));
      }

      Viewmodel.Groups.Remove(group);
    }

    protected override void ReverseActionMethod(string key)
    {
      new ReversibleAddGroupAction(Viewmodel).Action.Invoke(key);

      // Move the old items to the group
      foreach (var card in AffectedCards)
        new ReversibleCardGroupChangeAction(Viewmodel).Action.Invoke((card, key));
    }
  }
}