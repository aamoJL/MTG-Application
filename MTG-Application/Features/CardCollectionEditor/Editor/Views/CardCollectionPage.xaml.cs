using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.CardCollection;

public sealed partial class CardCollectionPage : Page
{
  public CardCollectionPage()
  {
    InitializeComponent();

    CardCollectionEditorViewDialogs.RegisterConfirmDialogs(ViewModel.Confirmers, () => new(XamlRoot));
    NotificationService.RegisterNotifications(ViewModel.Notifier, this);
    WindowClosing.Closing += WindowClosing_Closing;
    WindowClosing.Closed += WindowClosing_Closed;
  }

  public CardCollectionEditorViewModel ViewModel { get; } = new(new ScryfallAPI(), new(), new(), new CardCollectionDTORepository(), new());

  private async void NewCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.NewCollectionCommand.CanExecute(null))
      await ViewModel.NewCollectionCommand.ExecuteAsync(null);
  }

  private async void SaveCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.CardCollectionViewModel.SaveCollectionCommand.CanExecute(null))
      await ViewModel.CardCollectionViewModel.SaveCollectionCommand.ExecuteAsync(null);
  }

  private async void OpenCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.OpenCollectionCommand.CanExecute(null))
      await ViewModel.OpenCollectionCommand.ExecuteAsync(null);
  }

  private void WindowClosing_Closing(object sender, WindowClosing.ClosingEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    e.Tasks.Add(ViewModel.ConfirmUnsavedChangesCommand.ExecuteAsync);
  }

  private void WindowClosing_Closed(object sender, WindowClosing.ClosedEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    WindowClosing.Closing -= WindowClosing_Closing;
    WindowClosing.Closed -= WindowClosing_Closed;
  }
}