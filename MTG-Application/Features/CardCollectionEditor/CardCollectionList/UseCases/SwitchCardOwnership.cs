﻿using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class SwitchCardOwnership(CardCollectionListViewModel viewmodel) : ViewModelCommand<CardCollectionListViewModel, CardCollectionMTGCard>(viewmodel)
  {
    protected override bool CanExecute(CardCollectionMTGCard? card) => card != null;

    protected override void Execute(CardCollectionMTGCard? card)
    {
      if (!CanExecute(card))
        return;

      if (Viewmodel.CollectionList.Cards.FirstOrDefault(x => x.Info.ScryfallId == card!.Info.ScryfallId) is CardCollectionMTGCard existingCard)
        Viewmodel.CollectionList.Cards.Remove(existingCard);
      else
        Viewmodel.CollectionList.Cards.Add(card!);
    }
  }
}