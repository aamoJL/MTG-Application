using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard;

public partial class CardCollectionMTGCardViewModel(MTGCard card) : ViewModelBase
{
  public Worker Worker { private get; init; } = new();
  public IMTGCardImporter Importer { private get; init; } = App.MTGCardImporter;
  public Notifier Notifier { private get; init; } = new();
  public INetworkService NetworkService { private get; init; } = new NetworkService();
  public CollectionCardConfirmers Confirmers { private get; init; } = new();

  public MTGCardInfo Info => Card.Info;
  [ObservableProperty] public partial bool IsOwned { get; set; }

  private MTGCard Card { get; } = card;

  [RelayCommand]
  private async Task ShowPrints()
  {
    try
    {
      var prints = (await Worker.DoWork(new FetchCardPrints(Importer).Execute(Card.Info.PrintSearchUri))).Found;

      await Confirmers.ConfirmCardPrints(Confirmations.GetShowCardPrintsConfirmation(prints.Select(x => new MTGCard(x.Info))));
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
      await NetworkService.OpenUri(Card.Info.APIWebsiteUri);
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
      await NetworkService.OpenUri(Card.Info.CardMarketUri);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }
}
