using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRemoveCardsAction : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
  {
    public ReversibleRemoveCardsAction(IList<DeckEditorMTGCard> collection)
    {
      Collection = collection;
      Action = ActionMethod;
      ReverseAction = ReverseActionMethod;
    }

    private IList<DeckEditorMTGCard> Collection { get; }

    protected void ActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => Remove(cards);

    protected void ReverseActionMethod(IEnumerable<DeckEditorMTGCard> cards)
      => new ReversibleAddCardsAction(Collection).Action?.Invoke(cards);

    private void Remove(IEnumerable<DeckEditorMTGCard> cards)
    {
      foreach (var card in cards)
      {
        if (Collection.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
        {
          if (existingCard.Count <= card.Count)
            Collection.Remove(existingCard);
          else
            existingCard.Count -= card.Count;
        }
      }
    }
  }
}