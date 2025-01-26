using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddGroupAction(GroupedCardListViewModel viewmodel) : ViewModelReversibleAction<GroupedCardListViewModel, CardGroupViewModel>(viewmodel)
  {
    protected override void ActionMethod(CardGroupViewModel group)
    {
      if (group.Key == string.Empty
        || Viewmodel.Groups.FirstOrDefault(x => x.Key == group.Key) is not null)
        return;

      // Find the alphabetical index of the key. Empty key will always be the last item
      if (Viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty || x.Key.CompareTo(group.Key) >= 0) is CardGroupViewModel g
        && Viewmodel.Groups.IndexOf(g) is int i && i >= 0)
      {
        Viewmodel.Groups.Insert(i, group);
      }
      else
        Viewmodel.Groups.Add(group);
    }

    protected override void ReverseActionMethod(CardGroupViewModel group)
      => new ReversibleRemoveGroupAction(Viewmodel).Action?.Invoke(group);
  }
}