using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.UseCases;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardSearch.ViewModels.SearchCard;

public partial class CardSearchMTGCardViewModel(MTGCard card) : ViewModelBase
{
  public Worker Worker { get; init; } = new();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public Notifier Notifier { private get; init; } = new();
  public INetworkService NetworkService { private get; init; } = new NetworkService();
  public SearchCardConfirmers Confirmers { private get; init; } = new();

  public MTGCardInfo Info => Model.Info;

  private MTGCard Model { get; } = card;

  [RelayCommand]
  private async Task ShowCardPrints()
  {
    try
    {
      var prints = await Worker.DoWork(new FetchCardPrints(Importer).Execute(Model.Info.PrintSearchUri));

      await Confirmers.ConfirmCardPrints(new(Title: "Card prints", Message: string.Empty, Data: prints));
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  [RelayCommand]
  private async Task OpenAPIWebsite()
  {
    try
    {
      await NetworkService.OpenUri(Model.Info.APIWebsiteUri);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task OpenCardMarketWebsite()
  {
    try
    {
      await NetworkService.OpenUri(Model.Info.CardMarketUri);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }
}