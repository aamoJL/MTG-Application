using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MTGApplication.Features.DeckEditor;
[ObservableObject]
public sealed partial class DeckEditorPage : Page
{
  public DeckEditorPage()
  {
    InitializeComponent();

    DeckEditorViewDialogs.RegisterConfirmDialogs(ViewModel.DeckEditorConfirmers, () => new(XamlRoot));
    DeckEditorViewNotifications.RegisterNotifications(ViewModel.Notifier, this);
  }

  public DeckEditorViewModel ViewModel { get; } = new();

  [ObservableProperty] private bool deckImageViewVisible = true;
  [ObservableProperty] private bool deckTextViewVisible = false;

  [RelayCommand]
  private void SetDeckDisplayType(string type)
  {
    if (type == "Image")
    {
      DeckImageViewVisible = true;
      DeckTextViewVisible = false;
    }
    else if (type == "Text")
    {
      DeckImageViewVisible = false;
      DeckTextViewVisible = true;
    }
  }

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is string deckName)
    {
      if (ViewModel.OpenDeckCommand.CanExecute(deckName)) ViewModel.OpenDeckCommand.Execute(deckName);
    }
  }

  private void SaveDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.SaveDeckCommand.CanExecute(null)) ViewModel.SaveDeckCommand.Execute(null);
    args.Handled = true;
  }

  private void OpenDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.OpenDeckCommand.CanExecute(null)) ViewModel.OpenDeckCommand.Execute(null);
    args.Handled = true;
  }

  private void NewDeckKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.NewDeckCommand.CanExecute(null)) ViewModel.NewDeckCommand.Execute(null);
    args.Handled = true;
  }

  private void ResetFiltersKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.CardFilters.ResetCommand.CanExecute(null)) ViewModel.CardFilters.ResetCommand.Execute(null);
    args.Handled = true;
  }

  private void UndoKeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.UndoCommand.CanExecute(null)) ViewModel.UndoCommand.Execute(null);
    args.Handled = true;
  }

  private void RedoboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (ViewModel.RedoCommand.CanExecute(null)) ViewModel.RedoCommand.Execute(null);
    args.Handled = true;
  }
}