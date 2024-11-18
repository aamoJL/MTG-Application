using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddGroupAction(GroupedCardListViewModel viewmodel) : ViewModelReversibleAction<GroupedCardListViewModel, string>(viewmodel)
  {
    public CardGroupViewModel Group { get; set; }

    protected override void ActionMethod(string key)
    {
      if (Viewmodel.Groups.FirstOrDefault(x => x.Key == key) is CardGroupViewModel existing)
        return;

      Group ??= new CardGroupViewModel(key, Viewmodel);

      // Find the alphabetical index of the key. Empty key will always be the last item
      var index = Viewmodel.Groups.IndexOf(
              Viewmodel.Groups.FirstOrDefault(x => x.Key == string.Empty || x.Key.CompareTo(Group.Key) >= 0));

      if (index >= 0)
        Viewmodel.Groups.Insert(index, Group);
      else
        Viewmodel.Groups.Add(Group);
    }

    protected override void ReverseActionMethod(string key)
      => new ReversibleRemoveGroupAction(Viewmodel) { Group = Group }.Action.Invoke(Group.Key);
  }
}