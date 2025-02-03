using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public partial class CardSearchViewModelCommands
{
  public class SubmitSearch(CardSearchViewModel viewmodel) : ViewModelAsyncCommand<CardSearchViewModel, string>(viewmodel)
  {
    protected override async Task Execute(string? query)
    {
      try
      {
        var searchResult = await (Viewmodel as IWorker).DoWork(new FetchCardsWithSearchQuery(Viewmodel.Importer).Execute(query ?? string.Empty));

        await Viewmodel.Cards.SetCollection(
          cards: [.. searchResult.Found.Select(x => new MTGCard(x.Info))],
          nextPageUri: searchResult.NextPageUri,
          totalCount: searchResult.TotalCount);
      }
      catch (Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, e.Message));
      }
    }
  }
}
