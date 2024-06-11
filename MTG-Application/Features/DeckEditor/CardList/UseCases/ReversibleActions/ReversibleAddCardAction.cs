using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddCardAction(CardListViewModel viewmodel) : ViewModelReversibleAction<CardListViewModel, IEnumerable<DeckEditorMTGCard>>(viewmodel)
  {
    protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
    {
      var addList = new List<DeckEditorMTGCard>();

      foreach (var card in cards)
      {
        if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card?.Info.Name) is DeckEditorMTGCard existingCard)
          existingCard.Count += card.Count;
        else if (card != null)
          addList.Add(card);
      }

      foreach (var item in addList)
        Viewmodel.Cards.Add(item);

      Viewmodel.OnChange?.Invoke();
    }

    protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => new ReversibleRemoveCardAction(Viewmodel).Action.Invoke(cards);
  }
}