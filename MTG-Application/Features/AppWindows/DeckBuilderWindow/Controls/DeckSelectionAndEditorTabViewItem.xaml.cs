using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.Features.DeckEditor.Views;
using MTGApplication.Features.DeckSelection.Views;
using MTGApplication.General.ViewModels;
using MTGApplication.General.Views.AppWindows;
using System.Threading.Tasks;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
public sealed partial class DeckSelectionAndEditorTabViewItem : TabViewItem
{
  public DeckSelectionAndEditorTabViewItem()
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
  public async Task RequestClosure(ISavable.ConfirmArgs args)
  {
    await ConfirmClosure(args);

    if (args.Cancelled)
      return;

    Close();
  }

  private void Frame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
  {
    if (e.Content is DeckSelectionPage selectorPage)
    {
      selectorPage.DeckSelectedCommand = new RelayCommand<string>((selectedDeck) =>
      {
        Frame.Navigate(typeof(DeckEditorPage), selectedDeck ?? "", new SuppressNavigationTransitionInfo());
      });
    }
    else if (e.Content is DeckEditorPage deckEditor)
    {
      Header.Text = !string.IsNullOrEmpty(deckEditor.ViewModel.Name) ? deckEditor.ViewModel.Name : "New deck";

      deckEditor.ViewModel.PropertyChanged += DeckEditorPageViewModel_PropertyChanged;
    }
  }

  private void DeckEditorPageViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is DeckEditorViewModel deckEditorViewModel)
    {
      if (e.PropertyName == nameof(deckEditorViewModel.Name))
        Header.Text = !string.IsNullOrEmpty(deckEditorViewModel.Name) ? deckEditorViewModel.Name : "New deck";

      if (e.PropertyName == nameof(deckEditorViewModel.HasUnsavedChanges))
        Header.UnsavedIndicator = deckEditorViewModel.HasUnsavedChanges;
    }
  }

  private void WindowClosing_Closing(object? _, WindowClosing.ClosingEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    e.Tasks.Add(ConfirmClosure);
  }

  private void WindowClosing_Closed(object? _, WindowClosing.ClosedEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    Close();
  }

  private async Task ConfirmClosure(ISavable.ConfirmArgs args)
  {
    if (args.Cancelled) return;

    if (Frame?.Content is DeckEditorPage deckEditorPage && deckEditorPage.ViewModel.HasUnsavedChanges)
    {
      IsSelected = true; // Will bring this tab view to the front

      await deckEditorPage.ViewModel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);
    }
  }

  private void Close()
  {
    if (Frame?.Content is DeckEditorPage deckEditorPage)
      deckEditorPage.ViewModel.PropertyChanged -= DeckEditorPageViewModel_PropertyChanged;

    WindowClosing.Closing -= WindowClosing_Closing;
    WindowClosing.Closed -= WindowClosing_Closed;

    if (Frame != null)
      Frame.Navigated -= Frame_Navigated;
  }
}

