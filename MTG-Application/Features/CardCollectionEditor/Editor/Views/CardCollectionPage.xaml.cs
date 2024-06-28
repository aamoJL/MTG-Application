using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardCollection.Editor.Services;
using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.CardCollectionEditor.Editor.Views;

public sealed partial class CardCollectionPage : Page
{
  public CardCollectionPage()
  {
    InitializeComponent();

    Loaded += CardCollectionPage_Loaded;
  }

  private void CardCollectionPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
  {
    Loaded -= CardCollectionPage_Loaded;

    CardCollectionEditorViewDialogs.RegisterConfirmDialogs(ViewModel.Confirmers, wrapper: new(XamlRoot));
    NotificationService.RegisterNotifications(ViewModel.Notifier, this);
    WindowClosing.Closing += WindowClosing_Closing;
    WindowClosing.Closed += WindowClosing_Closed;
  }

  public CardCollectionEditorViewModel ViewModel { get; } = new(App.MTGCardImporter, new(), new(), new CardCollectionDTORepository(), new());

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