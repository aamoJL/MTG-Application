using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Services.NotificationService;

namespace MTGApplication.Features.CardCollection;
public sealed partial class CardCollectionPage : Page
{
  public CardCollectionPage()
  {
    InitializeComponent();
    
    CardCollectionViewDialogs.RegisterConfirmDialogs(ViewModel.Confirmers, () => new(XamlRoot));
    NotificationService.RegisterNotifications(ViewModel.Notifier, this);
  }

  public CardCollectionViewModel ViewModel { get; } = new(App.MTGCardAPI);
}
