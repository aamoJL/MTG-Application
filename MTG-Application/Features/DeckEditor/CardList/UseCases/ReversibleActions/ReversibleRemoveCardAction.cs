using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRemoveCardAction(CardListViewModel viewmodel) : ViewModelReversibleAction<CardListViewModel, IEnumerable<DeckEditorMTGCard>>(viewmodel)
  {
    public IEnumerable<DeckEditorMTGCard>? Cards { get; set; }

    protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => Remove(Cards ??= cards);

    protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => new ReversibleAddCardAction(Viewmodel).Action?.Invoke(Cards ??= cards);

    private void Remove(IEnumerable<DeckEditorMTGCard> cards)
    {
      foreach (var card in cards)
      {
        if (card == null)
          return;

        if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
        {
          if (existingCard.Count <= card.Count)
            Viewmodel.Cards.Remove(existingCard);
          else
            existingCard.Count -= card.Count;
        }
      }

      Viewmodel.OnListChange();
    }
  }
}