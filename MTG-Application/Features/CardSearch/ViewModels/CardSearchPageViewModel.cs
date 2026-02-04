using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.Models;
using MTGApplication.Features.CardSearch.UseCases;
using MTGApplication.Features.CardSearch.Views;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardSearch.ViewModels;

/// <summary>
/// ViewModel for <see cref="CardSearchPage"/>
/// </summary>
public partial class CardSearchPageViewModel(IMTGCardImporter importer) : ViewModelBase
{
  public IMTGCardImporter Importer { get; } = importer;
  public Notifier Notifier { get; init; } = new();
  public Worker Worker { get; set; } = new();
  public IncrementalLoadingCardCollection<MTGCard> Cards { get; } = new(new CardSearchIncrementalCardSource(importer));

  public Func<string, CancellationToken, Task<CardImportResult>> FetchCardsWithQuery_UC { private get => field ??= new FetchCards(Importer).Execute; set; }
  public Func<MTGCardInfo, Task<IEnumerable<MTGCard>>> FetchCardPrints_UC { private get => field ??= new FetchCardPrints(Importer).Execute; set; }
  public Func<Confirmation<IEnumerable<MTGCard>>, Task>? ConfirmCardPrints_UC { private get; set; }

  [RelayCommand]
  private async Task ShowCardPrints(MTGCard card)
  {
    try
    {
      var prints = await Worker.DoWork(() => FetchCardPrints_UC(card.Info));

      if (ConfirmCardPrints_UC != null)
        await ConfirmCardPrints_UC(new(Title: "Card prints", Message: string.Empty, Data: prints));
    }
    catch (Exception e)
    {
      new SendNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  [RelayCommand]
  private async Task SubmitSearch(string query, CancellationToken token)
  {
    try
    {
      await Worker.DoWork(async () =>
      {
        var result = await FetchCardsWithQuery_UC(query ?? string.Empty, token);

        token.ThrowIfCancellationRequested();

        await Cards.SetCollection(
            cards: [.. result.Found.Select(x => new MTGCard(x.Info))],
            nextPageUri: result.NextPageUri,
            totalCount: result.TotalCount);
      });
    }
    catch (OperationCanceledException) { }
    catch (Exception e)
    {
      new SendNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }
}