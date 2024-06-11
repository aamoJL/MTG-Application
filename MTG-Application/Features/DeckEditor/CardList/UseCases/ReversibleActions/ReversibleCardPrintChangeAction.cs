﻿using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardPrintChangeAction(CardListViewModel viewmodel) : ViewModelReversibleAction<CardListViewModel, (DeckEditorMTGCard Card, MTGCardInfo Info)>(viewmodel)
  {
    protected override void ActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
      => CardPrintChange(param.Card, param.Info);

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, MTGCardInfo Info) param)
      => CardPrintChange(param.Card, param.Info);

    private void CardPrintChange(DeckEditorMTGCard card, MTGCardInfo info)
    {
      if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        existingCard.Info = info with { };
        Viewmodel.OnChange?.Invoke();
      }
    }
  }
}