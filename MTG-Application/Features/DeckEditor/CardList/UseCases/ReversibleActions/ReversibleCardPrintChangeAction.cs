using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardPrintChangeAction(CardListViewModel viewmodel) : ViewModelReversibleAction<CardListViewModel, (DeckEditorMTGCard Card, MTGCardInfo Info)>(viewmodel)
  {
    public DeckEditorMTGCard? Card { get; set; }

    protected override void ActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
    {
      if ((Card ??= Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == param.Card.Info.Name)) is DeckEditorMTGCard card)
        CardPrintChange(card, param.Info);
    }

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
      => ActionMethod(param);

    private void CardPrintChange(DeckEditorMTGCard card, MTGCardInfo info)
    {
      card.Info = info;
      Viewmodel.OnCardChange(card, nameof(card.Info));
    }
  }
}