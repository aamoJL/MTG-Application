using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.UseCases;
using MTGApplication.Features.CardSearch.ViewModels.SearchCard;
using MTGApplication.Features.CardSearch.Views;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardSearch.ViewModels.SearchPage;

/// <summary>
/// ViewModel for <see cref="CardSearchPage"/>
/// </summary>
public partial class CardSearchPageViewModel : ViewModelBase
{
  public Worker Worker { get; init; } = new();
  public IMTGCardImporter Importer { get; init; } = App.MTGCardImporter;
  public Notifier Notifier { get; init; } = new();
  public NetworkService NetworkService { get; init; } = new();
  public CardSearchMTGCardViewModel.SearchCardConfirmers CardConfirmers { get; init; } = new();

  public IncrementalLoadingCardCollection<CardSearchMTGCardViewModel> QueryCards
  {
    get => field ??= QueryCards = CreateQueryCollection([], string.Empty, 0);
    private set => SetProperty(ref field, value);
  }

  [RelayCommand]
  private async Task SubmitSearch(string query, CancellationToken token)
  {
    try
    {
      await Worker.DoWork(async () =>
      {
        QueryCards = CreateQueryCollection(
          cards: [],
          nextPage: string.Empty,
          totalCount: 0);

        var result = await new FetchCards(Importer) { CancellationToken = token }.Execute(query ?? string.Empty);

        token.ThrowIfCancellationRequested();

        QueryCards = CreateQueryCollection(
          cards: [.. result.Found.Select(x => CreateCardViewModel(new(x.Info)))],
          nextPage: result.NextPageUri,
          totalCount: result.TotalCount);
      });
    }
    catch (OperationCanceledException) { }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  private IncrementalLoadingCardCollection<CardSearchMTGCardViewModel> CreateQueryCollection(IEnumerable<CardSearchMTGCardViewModel> cards, string nextPage, int totalCount)
  {
    var source = new IncrementalCardSource<CardSearchMTGCardViewModel>(Importer)
    {
      Cards = [.. cards],
      NextPage = nextPage,
      Converter = (item) => CreateCardViewModel(new(item.Info)),
      OnError = (e) => new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message)),
    };

    return new(source)
    {
      TotalCardCount = totalCount,
    };
  }

  private CardSearchMTGCardViewModel CreateCardViewModel(MTGCard model) => new(model)
  {
    Worker = Worker,
    Importer = Importer,
    Notifier = Notifier,
    Confirmers = CardConfirmers,
    NetworkService = NetworkService
  };
}