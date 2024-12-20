﻿using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleCardCountChangeAction(CardListViewModel viewmodel) : ViewModelReversibleAction<CardListViewModel, (DeckEditorMTGCard Card, int Value)>(viewmodel)
  {
    public DeckEditorMTGCard? Card { get; set; }

    protected override void ActionMethod((DeckEditorMTGCard Card, int Value) param)
    {
      if ((Card ??= Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == param.Card.Info.Name)) is DeckEditorMTGCard card)
        CountChange(card, param.Value);
    }

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, int Value) param)
      => ActionMethod(param);

    private void CountChange(DeckEditorMTGCard card, int value)
    {
      card.Count = value;
      Viewmodel.OnCardChange(card, nameof(card.Count));
    }
  }
}