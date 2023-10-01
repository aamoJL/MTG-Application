using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.API;
using MTGApplication.Database.Repositories;
using MTGApplication.Interfaces;
using MTGApplication.Services;
using MTGApplication.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI;
using static MTGApplication.Services.NotificationService;

namespace MTGApplication.Views.Pages;

/// <summary>
/// Page that shows MTG card collections
/// </summary>
public sealed partial class MTGCardCollectionPage : Page, ISavable, IDialogPresenter
{
  public MTGCardCollectionPage()
  {
    InitializeComponent();
    CardCollectionsViewModel = new(new ScryfallAPI(), new SQLiteMTGCardCollectionRepository(new ScryfallAPI(), new()), new());

    Loaded += MTGCardCollectionPage_Loaded;
    Unloaded += MTGCardCollectionPage_Unloaded;

    OnNotificationHandler = (s, args) => { RaiseNotification(XamlRoot, args); };
    OnGetDialogWrapperHandler = (s, args) => { args.DialogWrapper = DialogWrapper; };
  }

  public CardCollectionsViewModel CardCollectionsViewModel { get; }
  public DialogService.DialogWrapper DialogWrapper { get; private set; }

  private EventHandler<NotificationEventArgs> OnNotificationHandler { get; }
  private EventHandler<DialogService.DialogEventArgs> OnGetDialogWrapperHandler { get; }

  #region ISavable implementation
  public bool HasUnsavedChanges { get => CardCollectionsViewModel.HasUnsavedChanges; set => CardCollectionsViewModel.HasUnsavedChanges = value; }

  public async Task<bool> SaveUnsavedChanges() => await CardCollectionsViewModel.SaveUnsavedChanges();
  #endregion

  #region Events
  private void MTGCardCollectionPage_Loaded(object sender, RoutedEventArgs e)
  {
    DialogWrapper = new(XamlRoot);
    OnNotification += Notifications_OnNotification;
    CardCollectionsViewModel.OnNotification += OnNotificationHandler;
    CardCollectionsViewModel.OnGetDialogWrapper += OnGetDialogWrapperHandler;
  }

  private void MTGCardCollectionPage_Unloaded(object sender, RoutedEventArgs e)
  {
    OnNotification -= Notifications_OnNotification;
    CardCollectionsViewModel.OnNotification -= OnNotificationHandler;
    CardCollectionsViewModel.OnGetDialogWrapper -= OnGetDialogWrapperHandler;
  }

  private void Notifications_OnNotification(object sender, NotificationEventArgs e)
  {
    if ((XamlRoot)sender == XamlRoot)
    {
      InAppNotification.Background = e.Type switch
      {
        NotificationType.Error => new SolidColorBrush(Color.FromArgb(255, 248, 215, 218)),
        NotificationType.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 243, 205)),
        NotificationType.Success => new SolidColorBrush(Color.FromArgb(255, 212, 237, 218)),
        _ => new SolidColorBrush(Color.FromArgb(255, 204, 229, 255)),
      };
      InAppNotification.RequestedTheme = ElementTheme.Light;
      InAppNotification.Show(e.Text, NotificationDuration);
    }
  }
  #endregion

  #region Pointer events
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
  #endregion
}
