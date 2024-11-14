using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardGroupChangeAction(CardListViewModel viewmodel) : ViewModelReversibleAction<CardListViewModel, (DeckEditorMTGCard Card, string Group)>(viewmodel)
  {
    protected override void ActionMethod((DeckEditorMTGCard Card, string Group) param)
      => GroupChange(param.Card, param.Group);

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, string Group) param)
      => GroupChange(param.Card, param.Group);

    private void GroupChange(DeckEditorMTGCard card, string group)
    {
      if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        existingCard.Group = group;
        Viewmodel.OnCardChange(existingCard, nameof(existingCard.Group));
      }
    }
  }
}