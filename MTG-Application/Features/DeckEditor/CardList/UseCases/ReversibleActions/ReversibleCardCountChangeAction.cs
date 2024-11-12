using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardCountChangeAction(CardListViewModel viewmodel) : ViewModelReversibleAction<CardListViewModel, (DeckEditorMTGCard Card, int Value)>(viewmodel)
  {
    protected override void ActionMethod((DeckEditorMTGCard Card, int Value) param)
      => CountChange(param.Card, param.Value);

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, int Value) param)
      => CountChange(param.Card, param.Value);

    private void CountChange(DeckEditorMTGCard card, int value)
    {
      if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        existingCard.Count = value;
        Viewmodel.OnCardChange(existingCard, nameof(existingCard.Count));
      }
    }
  }
}