using MTGApplication.Features.EdhrecSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.EdhrecSearch.UseCases;

public class ChangeCommanderTheme(EdhrecSearchPageViewModel viewmodel) : AsyncCommand<EdhrecImporter.CommanderTheme>
{
  public EdhrecSearchPageViewModel Viewmodel { get; } = viewmodel;

  protected override async Task Execute(EdhrecImporter.CommanderTheme theme)
  {
    try
    {
      var query = string.Join(Environment.NewLine,
        await (Viewmodel as IWorker).DoWork(EdhrecImporter.FetchNewCardNames(theme.Uri)));

      var searchResult = await (Viewmodel as IWorker).DoWork(Viewmodel.Importer.ImportWithString(query));

      await Viewmodel.Cards.SetCollection(
          cards: [.. searchResult.Found.Select(x => new MTGCard(x.Info))],
          nextPageUri: searchResult.NextPageUri,
          totalCount: searchResult.TotalCount);
    }
    catch (Exception e)
    {
      Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
    }
  }
}
