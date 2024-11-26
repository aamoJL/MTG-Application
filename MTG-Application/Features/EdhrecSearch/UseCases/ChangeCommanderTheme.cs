using MTGApplication.Features.EdhrecSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;

namespace MTGApplication.Features.EdhrecSearch.UseCases;

public class ChangeCommanderTheme(EdhrecSearchPageViewModel viewmodel) : ViewModelAsyncCommand<EdhrecSearchPageViewModel, CommanderTheme>(viewmodel)
{
  protected override async Task Execute(CommanderTheme theme)
  {
    try
    {
      var query = string.Join(Environment.NewLine,
        await Viewmodel.Worker.DoWork(FetchNewCardNames(theme.Uri)));

      var searchResult = await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromString(query));

      Viewmodel.SelectedTheme = theme;
      Viewmodel.Cards.SetCollection(
          cards: [.. searchResult.Found.Select(x => new MTGCard(x.Info))],
          nextPageUri: searchResult.NextPageUri,
          totalCount: searchResult.TotalCount);
    }
    catch { }
  }
}
