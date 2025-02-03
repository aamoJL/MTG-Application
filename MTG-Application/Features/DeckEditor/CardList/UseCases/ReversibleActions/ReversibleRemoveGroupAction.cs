using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRemoveGroupAction(GroupedCardListViewModel viewmodel) : ViewModelReversibleAction<GroupedCardListViewModel, CardGroupViewModel>(viewmodel)
  {
    private DeckEditorMTGCard[]? AffectedCards { get; set; }

    protected override void ActionMethod(CardGroupViewModel group)
    {
      if (group == null)
        return;

      RemoveGroup(group);
    }

    protected override void ReverseActionMethod(CardGroupViewModel group)
    {
      new ReversibleAddGroupAction(Viewmodel).Action?.Invoke(group);

      // Move old cards back
      if (AffectedCards != null)
        foreach (var card in AffectedCards)
          new ReversibleCardGroupChangeAction().Action?.Invoke((card, group.Key));
    }

    private void RemoveGroup(CardGroupViewModel group)
    {
      AffectedCards ??= [.. group.Cards];

      // Move cards to default group
      foreach (var card in AffectedCards)
        new ReversibleCardGroupChangeAction().Action?.Invoke((card, string.Empty));

      Viewmodel.Groups.Remove(group);
    }
  }
}