using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddCardAction(CardListViewModel viewmodel) : ViewModelReversibleAction<CardListViewModel, IEnumerable<DeckEditorMTGCard>>(viewmodel)
  {
    public IEnumerable<DeckEditorMTGCard>? Cards { get; set; }

    protected override void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => AddCards(Cards ??= cards);

    protected override void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => new ReversibleRemoveCardAction(Viewmodel).Action?.Invoke(Cards ??= cards);

    private void AddCards(IEnumerable<DeckEditorMTGCard> cards)
    {
      foreach (var card in cards)
      {
        if (card == null)
          return;

        if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
          existingCard.Count += card.Count;
        else
          Viewmodel.Cards.Add(card);
      }

      Viewmodel.OnListChange();
    }
  }
}