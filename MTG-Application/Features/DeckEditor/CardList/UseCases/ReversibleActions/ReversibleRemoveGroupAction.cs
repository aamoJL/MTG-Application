using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRemoveGroupAction(GroupedCardListViewModel viewmodel) : ViewModelReversibleAction<GroupedCardListViewModel, string>(viewmodel)
  {
    public CardGroupViewModel? Group { get; set; }

    private DeckEditorMTGCard[] AffectedCards { get; set; } = [];

    protected override void ActionMethod(string key)
    {
      if (string.IsNullOrEmpty(key))
        return;

      Group ??= Viewmodel.Groups.FirstOrDefault(x => x.Key == key);

      if (Group != null)
        RemoveGroup(Group);
    }

    protected override void ReverseActionMethod(string key)
    {
      if (Group == null)
        return;

      // Add old group back
      new ReversibleAddGroupAction(Viewmodel)
      {
        Group = Group
      }.Action?.Invoke(Group.Key);

      // Move old cards back
      foreach (var card in AffectedCards)
      {
        new ReversibleCardGroupChangeAction(Viewmodel)
        {
          Card = card
        }.Action?.Invoke((card, key));
      }
    }

    private void RemoveGroup(CardGroupViewModel group)
    {
      if (group == null)
        return;

      AffectedCards = [.. group.Cards];

      // Move cards to default group
      foreach (var card in AffectedCards)
      {
        new ReversibleCardGroupChangeAction(Viewmodel)
        {
          Card = card
        }.Action?.Invoke((card, string.Empty));
      }

      group.Cards.Clear();

      Viewmodel.Groups.Remove(group);
    }
  }
}