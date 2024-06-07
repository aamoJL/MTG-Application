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

  private async void NewCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.NewCollectionCommand.CanExecute(null))
      await ViewModel.NewCollectionCommand.ExecuteAsync(null);
  }

  private async void SaveCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.SaveCollectionCommand.CanExecute(null))
      await ViewModel.SaveCollectionCommand.ExecuteAsync(null);
  }

  private async void OpenCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.OpenCollectionCommand.CanExecute(null))
      await ViewModel.OpenCollectionCommand.ExecuteAsync(null);
  }
}