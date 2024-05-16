using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckSelector;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow;
/// <summary>
/// Use case to create new tab item with <see cref=""/> as the content
/// </summary>
public class CreateNewDeckViewTabItem : UseCase<DeckEditorTabViewItem>
{
  private readonly DeckEditorTabViewItem tabItem = new();

  public override DeckEditorTabViewItem Execute()
  {
    tabItem.Frame.Navigated += TabFrame_Navigated;
    tabItem.CloseRequested += TabItem_CloseRequested;

    tabItem.Frame.Navigate(typeof(DeckSelectorPage));

    return tabItem;
  }

  private void TabFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
  {
    if (e.Content is DeckSelectorPage selectorPage)
    {
      selectorPage.DeckSelectedCommand = new RelayCommand<string>((string selectedDeck) =>
      {
        tabItem.Frame.Navigate(typeof(DeckEditorPage), selectedDeck ?? "", new SuppressNavigationTransitionInfo());
      });
    }
    else if (e.Content is DeckEditorPage deckEditor)
    {
      tabItem.Header.Text = !string.IsNullOrEmpty(deckEditor.ViewModel.DeckName) ? deckEditor.ViewModel.DeckName : "New deck";

      deckEditor.ViewModel.PropertyChanged += DeckEditorPageViewModel_PropertyChanged;
    }
  }

  private void DeckEditorPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is DeckEditorViewModel deckEditorViewModel)
    {
      if (e.PropertyName == nameof(deckEditorViewModel.DeckName))
        tabItem.Header.Text = !string.IsNullOrEmpty(deckEditorViewModel.DeckName) ? deckEditorViewModel.DeckName : "New deck";

      if (e.PropertyName == nameof(deckEditorViewModel.HasUnsavedChanges))
        tabItem.Header.UnsavedIndicator = deckEditorViewModel.HasUnsavedChanges;
    }
  }

  private void TabItem_CloseRequested(Microsoft.UI.Xaml.Controls.TabViewItem sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
  {
    tabItem.CloseRequested -= TabItem_CloseRequested;
    tabItem.Frame.Navigated -= TabFrame_Navigated;

    if (tabItem.Frame.Content is DeckEditorPage deckEditorPage)
      deckEditorPage.ViewModel.PropertyChanged -= DeckEditorPageViewModel_PropertyChanged;
  }
}
