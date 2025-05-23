using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;
using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.AppWindows;
using System.Windows.Input;

namespace MTGApplication.Features.CardCollectionEditor.Editor.Views;

public sealed partial class CardCollectionPage : Page
{
  public CardCollectionPage()
  {
    InitializeComponent();

    Loaded += CardCollectionPage_Loaded;
  }

  private void CardCollectionPage_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= CardCollectionPage_Loaded;

    CardCollectionEditorViewDialogs.RegisterConfirmDialogs(ViewModel.Confirmers, root: XamlRoot);
    NotificationService.RegisterNotifications(ViewModel.Notifier, this);
    WindowClosing.Closing += WindowClosing_Closing;
    WindowClosing.Closed += WindowClosing_Closed;
  }

  public CardCollectionEditorViewModel ViewModel { get; } = new(App.MTGCardImporter);

  private ICommand SwitchWindowThemeCommand { get; } = new RelayCommand(
    execute: () => new ChangeWindowTheme(AppConfig.LocalSettings.AppTheme == ElementTheme.Dark
      ? ElementTheme.Light : ElementTheme.Dark).Execute());

  private async void NewCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.NewCollectionCommand?.CanExecute(null) is true)
      await ViewModel.NewCollectionCommand.ExecuteAsync(null);
  }

  private async void SaveCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.SaveCollectionCommand?.CanExecute(null) is true)
      await ViewModel.SaveCollectionCommand.ExecuteAsync(null);
  }

  private async void OpenCollectionKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.OpenCollectionCommand?.CanExecute(null) is true)
      await ViewModel.OpenCollectionCommand.ExecuteAsync(null);
  }

  private void WindowClosing_Closing(object? _, WindowClosing.ClosingEventArgs e)
  {
    if (e.Root != XamlRoot)
      return;

    if (ViewModel.ConfirmUnsavedChangesCommand != null)
      e.Tasks.Add(ViewModel.ConfirmUnsavedChangesCommand.ExecuteAsync);
  }

  private void WindowClosing_Closed(object? _, WindowClosing.ClosedEventArgs e)
  {
    if (e.Root != XamlRoot)
      return;

    WindowClosing.Closing -= WindowClosing_Closing;
    WindowClosing.Closed -= WindowClosing_Closed;
  }
}