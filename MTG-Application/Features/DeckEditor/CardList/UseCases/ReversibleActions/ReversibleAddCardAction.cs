using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleAddCardsAction : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
  {
    public ReversibleAddCardsAction(IList<DeckEditorMTGCard> collection)
    {
      Collection = collection;
      Action = ActionMethod;
      ReverseAction = ReverseActionMethod;
    }

    private IList<DeckEditorMTGCard> Collection { get; }

    protected void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => AddCards(cards);

    protected void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => new ReversibleRemoveCardsAction(Collection).Action?.Invoke(cards);

    private void AddCards(IEnumerable<DeckEditorMTGCard> cards)
    {
      foreach (var card in cards)
      {
        if (Collection.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
          existingCard.Count += card.Count;
        else
          Collection.Add(card);
      }
    }
  }
}