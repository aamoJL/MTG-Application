using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardGroupChangeAction(GroupedCardListViewModel viewmodel) : ViewModelReversibleAction<GroupedCardListViewModel, (DeckEditorMTGCard Card, string Group)>(viewmodel)
  {
    public DeckEditorMTGCard Card { get; set; }

    protected override void ActionMethod((DeckEditorMTGCard Card, string Group) param)
      => GroupChange(Card ??= Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == param.Card.Info.Name), param.Group);

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, string Group) param)
      => GroupChange(Card ??= Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == param.Card.Info.Name), param.Group);

    private void GroupChange(DeckEditorMTGCard card, string key)
    {
      if (card == null || card.Group == key)
        return;

      // remove card if exists in the old group
      if (Viewmodel.Groups.FirstOrDefault(x => x.Key == card.Group) is CardGroupViewModel oldGroup)
        oldGroup.Items.Remove(card);

      // Change group
      card.Group = key;

      // If the card is still in the list, add the card to the new group
      if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) != null)
      {
        if (Viewmodel.Groups.FirstOrDefault(x => x.Key == key) is not CardGroupViewModel newGroup)
        {
          // Create new group if does not exist
          var addAction = new ReversibleAddGroupAction(Viewmodel);
          addAction.Action.Invoke(key);
          newGroup = addAction.Group;
        }

        // Add card to the new group
        newGroup.Items.Add(card);

        Viewmodel.OnCardChange(card, nameof(card.Group));
      }
    }
  }
}