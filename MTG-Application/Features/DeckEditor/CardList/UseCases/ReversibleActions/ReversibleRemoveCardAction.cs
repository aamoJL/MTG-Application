using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRemoveCardAction : ReversibleAction<IEnumerable<DeckEditorMTGCard>>
  {
    public ReversibleRemoveCardAction(CardListViewModel viewmodel)
    {
      Viewmodel = viewmodel;

      Action = Remove;
      ReverseAction = Add;
    }

    public CardListViewModel Viewmodel { get; }

    private void Remove(IEnumerable<DeckEditorMTGCard> cards)
    {
      var removeList = new List<DeckEditorMTGCard>();

      foreach (var card in cards)
      {
        if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card?.Info.Name) is DeckEditorMTGCard existingCard)
        {
          if (existingCard.Count <= card.Count) removeList.Add(existingCard);
          else existingCard.Count -= card.Count;
        }
      }

      foreach (var item in removeList)
        Viewmodel.Cards.Remove(item);

      Viewmodel.OnChange?.Invoke();
    }

    private void Add(IEnumerable<DeckEditorMTGCard> cards)
      => new ReversibleAddCardAction(Viewmodel).Action.Invoke(cards);
  }
}