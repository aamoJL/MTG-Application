using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddGroupAction(GroupedCardListViewModel viewmodel) : ViewModelReversibleAction<GroupedCardListViewModel, string>(viewmodel)
  {
    public CardGroupViewModel? Group { get; set; }

    protected override void ActionMethod(string key)
    {
      if (Viewmodel.Groups.FirstOrDefault(x => x.Key == key) is not null)
        return;

      Group ??= new CardGroupViewModel(key, Viewmodel);

      // Find the alphabetical index of the key. Empty key will always be the last item
      if (Viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty || x.Key.CompareTo(Group.Key) >= 0) is CardGroupViewModel group
        && Viewmodel.Groups.IndexOf(group) is int index && index >= 0)
      {
        Viewmodel.Groups.Insert(index, Group);
      }
      else
        Viewmodel.Groups.Add(Group);
    }

    protected override void ReverseActionMethod(string key)
    {
      if (Group != null)
        new ReversibleRemoveGroupAction(Viewmodel) { Group = Group }.Action?.Invoke(Group.Key);
    }
  }
}