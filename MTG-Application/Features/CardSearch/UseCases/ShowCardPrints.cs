﻿using MTGApplication.Features.CardSearch.Services;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public partial class CardSearchViewModelCommands
{
  public class ShowCardPrints(CardSearchViewModel viewmodel) : ViewModelAsyncCommand<CardSearchViewModel, MTGCard>(viewmodel)
  {
    protected override async Task Execute(MTGCard card)
    {
      if (card == null) return;
      var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => new MTGCard(x.Info));

      await Viewmodel.Confirmers.ShowCardPrintsConfirmer.Confirm(CardSearchConfirmers.GetShowCardPrintsConfirmation(prints));
    }
  }
}
