using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.ViewModels.SearchCard;
using MTGApplication.Features.EdhrecSearch.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.EdhrecSearch.ViewModels;

public partial class EdhrecSearchPageViewModel : ViewModelBase
{
  public Worker Worker { get; init; } = new();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public IEdhrecImporter EdhrecImporter { private get; init; } = new EdhrecImporter();
  public Notifier Notifier { private get; init; } = new();
  public CardSearchMTGCardViewModel.SearchCardConfirmers CardConfirmers { private get; init; } = new();

  public CommanderTheme[] CommanderThemes { get; set; } = [];
  public IncrementalLoadingCardCollection<CardSearchMTGCardViewModel> QueryCards
  {
    get => field ??= QueryCards = CreateQueryCollection([], string.Empty, 0);
    protected set => SetProperty(ref field, value);
  }

  protected CardSearchMTGCardViewModel.Factory CardViewModelFactory => field ??= new()
  {
    Worker = Worker,
    Importer = Importer,
    Notifier = Notifier,
    CardConfirmers = CardConfirmers
  };

  [RelayCommand]
  private async Task SelectCommanderTheme(CommanderTheme theme, CancellationToken token)
  {
    try
    {
      await Worker.DoWork(async () =>
      {
        QueryCards = CreateQueryCollection(
          cards: [],
          nextPage: string.Empty,
          totalCount: 0);

        var searchResult = await new FetchCardsByTheme(Importer, EdhrecImporter).Execute(theme);

        token.ThrowIfCancellationRequested();

        QueryCards = CreateQueryCollection(
            cards: [.. searchResult.Found.Select(x => CardViewModelFactory.Build(new(x.Info)))],
            nextPage: searchResult.NextPageUri,
            totalCount: searchResult.TotalCount);
      });
    }
    catch (OperationCanceledException) { }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  protected IncrementalLoadingCardCollection<CardSearchMTGCardViewModel> CreateQueryCollection(IEnumerable<CardSearchMTGCardViewModel> cards, string nextPage, int totalCount)
  {
    var source = new IncrementalCardSource<CardSearchMTGCardViewModel>(Importer)
    {
      Cards = [.. cards],
      NextPage = nextPage,
      Converter = (item) => CardViewModelFactory.Build(new(item.Info)),
      OnError = (e) => new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message)),
    };

    return new(source)
    {
      TotalCardCount = totalCount,
    };
  }
}