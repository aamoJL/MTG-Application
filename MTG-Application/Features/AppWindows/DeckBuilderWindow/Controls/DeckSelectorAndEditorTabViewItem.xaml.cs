using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckSelector;
using MTGApplication.General.ViewModels;
using MTGApplication.General.Views.AppWindows;
using System.Threading.Tasks;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
public sealed partial class DeckSelectorAndEditorTabViewItem : TabViewItem
{
  public DeckSelectorAndEditorTabViewItem()
  {
    InitializeComponent();

    WindowClosing.Closing += WindowClosing_Closing;
    WindowClosing.Closed += WindowClosing_Closed;
    Frame.Navigated += Frame_Navigated;

    Frame.Navigate(typeof(DeckSelectionPage));
  }

  public new TabHeader Header { get; set; } = new TabHeader() { Text = "New tab" };
  public Frame Frame => ContentFrame;

  /// <returns><see langword="true"/> if the tab can be closed; otherwise <see langword="false"/></returns>
  public async Task<bool> RequestClosure()
  {
    if (Frame?.Content is DeckEditorPage deckEditorPage && deckEditorPage.ViewModel.HasUnsavedChanges)
    {
      IsSelected = true; // Will bring this tab view to the front

      var unsavedArgs = new ISavable.ConfirmArgs();

      await deckEditorPage.ViewModel.ConfirmUnsavedChangesCommand.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Canceled)
        return false; // Closing cancelled
    }

    return true;
  }

  private void Frame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
  {
    if (e.Content is DeckSelectionPage selectorPage)
    {
      selectorPage.DeckSelectedCommand = new RelayCommand<string>((string selectedDeck) =>
      {
        Frame.Navigate(typeof(DeckEditorPage), selectedDeck ?? "", new SuppressNavigationTransitionInfo());
      });
    }
    else if (e.Content is DeckEditorPage deckEditor)
    {
      Header.Text = !string.IsNullOrEmpty(deckEditor.ViewModel.DeckName) ? deckEditor.ViewModel.DeckName : "New deck";

      deckEditor.ViewModel.PropertyChanged += DeckEditorPageViewModel_PropertyChanged;
    }
  }

  private void DeckEditorPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is DeckEditorViewModel deckEditorViewModel)
    {
      if (e.PropertyName == nameof(deckEditorViewModel.DeckName))
        Header.Text = !string.IsNullOrEmpty(deckEditorViewModel.DeckName) ? deckEditorViewModel.DeckName : "New deck";

      if (e.PropertyName == nameof(deckEditorViewModel.HasUnsavedChanges))
        Header.UnsavedIndicator = deckEditorViewModel.HasUnsavedChanges;
    }
  }

  private void WindowClosing_Closing(object sender, WindowClosing.ClosingEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    e.Tasks.Add(RequestClosure);
  }

  private void WindowClosing_Closed(object sender, WindowClosing.ClosedEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    if (Frame?.Content is DeckEditorPage deckEditorPage)
      deckEditorPage.ViewModel.PropertyChanged -= DeckEditorPageViewModel_PropertyChanged;

    WindowClosing.Closing -= WindowClosing_Closing;
    WindowClosing.Closed -= WindowClosing_Closed;
    Frame.Navigated -= Frame_Navigated;
  }
}

