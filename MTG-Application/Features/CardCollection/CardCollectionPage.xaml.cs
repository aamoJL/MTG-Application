using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.NotificationService;
using System.Linq;

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

  private async void CollectionListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.FirstOrDefault() is MTGCardCollectionList list)
      if (ViewModel.SelectListCommand.CanExecute(list.Name))
        await ViewModel.SelectListCommand.ExecuteAsync(list.Name);
  }
}