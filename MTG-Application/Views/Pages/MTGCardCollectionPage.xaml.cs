using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.API;
using MTGApplication.Database.Repositories;
using MTGApplication.Interfaces;
using MTGApplication.Services;
using MTGApplication.ViewModels;
using System.Threading.Tasks;
using Windows.UI;

namespace MTGApplication.Views.Pages;

/// <summary>
/// Page that shows MTG card collections
/// </summary>
public sealed partial class MTGCardCollectionPage : Page, ISavable
{
  public MTGCardCollectionPage(FrameworkElement dialogRoot = null)
  {
    InitializeComponent();
    var dialogService = new DialogService();
    CardCollectionsViewModel = new(new ScryfallAPI(), new SQLiteMTGCardCollectionRepository(new ScryfallAPI(), new()), new(dialogService));
    CardCollectionsViewModel.OnNotification += (s, args) => NotificationService.RaiseNotification(XamlRoot, args);

    NotificationService.OnNotification += Notifications_OnNotification;

    // Set dialog root
    if (dialogRoot != null)
      dialogRoot.Loaded += (s, e) => { dialogService.XamlRoot = dialogRoot.XamlRoot; };
    else
      Loaded += (s, e) => { dialogService.XamlRoot = XamlRoot; };
  }

  public CardCollectionsViewModel CardCollectionsViewModel { get; }

  private void Notifications_OnNotification(object sender, NotificationService.NotificationEventArgs e)
  {
    if ((XamlRoot)sender == this.XamlRoot)
    {
      InAppNotification.Background = e.Type switch
      {
        NotificationService.NotificationType.Error => new SolidColorBrush(Color.FromArgb(255, 248, 215, 218)),
        NotificationService.NotificationType.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 243, 205)),
        NotificationService.NotificationType.Success => new SolidColorBrush(Color.FromArgb(255, 212, 237, 218)),
        _ => new SolidColorBrush(Color.FromArgb(255, 204, 229, 255)),
      };
      InAppNotification.RequestedTheme = ElementTheme.Light;
      InAppNotification.Show(e.Text, NotificationService.NotificationDuration);
    }
  }

  private void GridViewItemImage_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
  {
    if (SingleTapSelectionModeSwitch.IsChecked is true) { return; }
    
    if (sender is FrameworkElement element && element.DataContext is MTGCardCollectionCardViewModel vm)
    {
      if (vm.IsOwned) 
      { 
        CardCollectionsViewModel.SelectedList.RemoveFromList(vm.Model); 
      }
      else 
      { 
        CardCollectionsViewModel.SelectedList.AddToList(vm.Model); 
      }
    }
  }

  private void GridViewItemImage_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
  {
    if (SingleTapSelectionModeSwitch.IsChecked is false) { return; }
    
    if (sender is FrameworkElement element && element.DataContext is MTGCardCollectionCardViewModel vm)
    {
      if (vm.IsOwned) 
      { 
        CardCollectionsViewModel.SelectedList.RemoveFromList(vm.Model); 
      }
      else 
      { 
        CardCollectionsViewModel.SelectedList.AddToList(vm.Model); 
      }
    }
  }
  #region ISavable implementation
  public bool HasUnsavedChanges { get => CardCollectionsViewModel.HasUnsavedChanges; set => CardCollectionsViewModel.HasUnsavedChanges = value; }

  public async Task<bool> SaveUnsavedChanges() => await CardCollectionsViewModel.SaveUnsavedChanges();
  #endregion
}
