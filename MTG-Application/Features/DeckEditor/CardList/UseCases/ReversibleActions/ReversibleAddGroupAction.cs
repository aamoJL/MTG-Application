using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddGroupAction(GroupedCardListViewModel viewmodel) : ViewModelReversibleAction<GroupedCardListViewModel, string>(viewmodel)
  {
    protected override void ActionMethod(string key)
      => AddNewGroup(key);

    protected override void ReverseActionMethod(string key)
      => new ReversibleRemoveGroupAction(Viewmodel).Action.Invoke(key);

    public CardGroupViewModel AddNewGroup(string key)
    {
      // Find the alphabetical index of the key. Empty key will always be the last item
      var index = Viewmodel.Groups.IndexOf(
              Viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty || x.Key.CompareTo(key) >= 0));

      var group = new CardGroupViewModel(key);
      group.Commands = new(group, Viewmodel);

      if (index >= 0)
        Viewmodel.Groups.Insert(index, group);
      else
        Viewmodel.Groups.Add(group);

      return group;
    }
  }
}