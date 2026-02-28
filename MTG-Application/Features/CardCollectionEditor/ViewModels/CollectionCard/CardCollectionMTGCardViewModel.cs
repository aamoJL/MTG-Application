using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard;

public partial class CardCollectionMTGCardViewModel(MTGCard card) : ViewModelBase
{
  public required Worker Worker { private get; init; }
  public required IMTGCardImporter Importer { private get; init; }
  public required Notifier Notifier { private get; init; }
  public required INetworkService NetworkService { private get; init; }
  public required CollectionCardConfirmers Confirmers { private get; init; }

  public MTGCardInfo Info => Model.Info;
  [ObservableProperty] public partial bool IsOwned { get; set; }

  private MTGCard Model { get; } = card;

  [RelayCommand]
  private async Task ShowPrints()
  {
    try
    {
      var prints = (await Worker.DoWork(new FetchCardPrints(Importer).Execute(Model.Info.PrintSearchUri)));

      await Confirmers.ConfirmCardPrints(Confirmations.GetShowCardPrintsConfirmation(prints));
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private void SwitchOwnership() => IsOwned = !IsOwned;

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
