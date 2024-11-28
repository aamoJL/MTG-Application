using MTGApplication.Features.CardSearch.Services;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public partial class CardSearchViewModelCommands
{
  public class ShowCardPrints(CardSearchViewModel viewmodel) : ViewModelAsyncCommand<CardSearchViewModel, MTGCard>(viewmodel)
  {
    protected override async Task Execute(MTGCard? card)
    {
      try
      {
        ArgumentNullException.ThrowIfNull(card);

        var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => new MTGCard(x.Info));

        await Viewmodel.Confirmers.ShowCardPrintsConfirmer.Confirm(CardSearchConfirmers.GetShowCardPrintsConfirmation(prints));
      }
      catch (Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}
