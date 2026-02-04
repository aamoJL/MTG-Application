using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.Features.EdhrecSearch.UseCases;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Services.NotificationService.UseCases;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;

namespace MTGApplication.Features.EdhrecSearch.ViewModels;

public partial class EdhrecSearchPageViewModel(IMTGCardImporter importer) : CardSearchPageViewModel(importer)
{
  public CommanderTheme[] CommanderThemes { get; set; } = [];

  public Func<CommanderTheme, Task<CardImportResult>> FetchCardsWithTheme_UC { private get => field ??= new FetchCards(Importer).Execute; set; }

  [RelayCommand]
  private async Task SelectCommanderTheme(CommanderTheme theme, CancellationToken token)
  {
    try
    {
      await Worker.DoWork(async () =>
      {
        await Cards.SetCollection(
          cards: [],
          nextPageUri: string.Empty,
          totalCount: 0);

        var searchResult = await FetchCardsWithTheme_UC(theme);

        token.ThrowIfCancellationRequested();

        await Cards.SetCollection(
            cards: [.. searchResult.Found.Select(x => new MTGCard(x.Info))],
            nextPageUri: searchResult.NextPageUri,
            totalCount: searchResult.TotalCount);
      });
    }
    catch (OperationCanceledException) { }
    catch (Exception e)
    {
      new SendNotification(Notifier).Execute(new(NotificationService.NotificationType.Error, $"Error: {e.Message}"));
    }
  }
}