using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckSelector;
using MTGApplication.General.ViewModels;
using MTGApplication.General.Views.Controls;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow;
/// <summary>
/// Use case to create new tab item with <see cref=""/> as the content
/// </summary>
public class CreateNewDeckViewTabItem : UseCase<CustomTabViewItem>
{
  private readonly Frame tabFrame = new();
  private readonly TabHeader tabHeader = new() { Text = "Deck selection" };
  private readonly CustomTabViewItem tabItem = new();

  public CreateNewDeckViewTabItem()
  {
    tabItem.Header = tabHeader;
    tabItem.Frame = tabFrame;
  }

  public override CustomTabViewItem Execute()
  {
    tabFrame.Content = new DeckSelectorPage()
    {
      DeckSelectedCommand = new RelayCommand<string>((string selectedDeck) =>
      {
        tabFrame.Navigate(typeof(DeckEditorPage), selectedDeck ?? "", new SuppressNavigationTransitionInfo());
      }),
    };

    tabFrame.Navigated += TabFrame_Navigated;
    tabItem.OnClosed += TabItem_OnClosed;

    return tabItem;
  }

  private void TabFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
  {
    if (e.Content is DeckEditorPage deckEditor)
    {
      tabHeader.Text = !string.IsNullOrEmpty(deckEditor.ViewModel.DeckName) ? deckEditor.ViewModel.DeckName : "New deck";

      deckEditor.ViewModel.PropertyChanged += DeckEditorPageViewModel_PropertyChanged;
    }
  }

  private void DeckEditorPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is DeckEditorViewModel deckEditorViewModel)
    {
      if (e.PropertyName == nameof(deckEditorViewModel.DeckName))
        tabHeader.Text = !string.IsNullOrEmpty(deckEditorViewModel.DeckName) ? deckEditorViewModel.DeckName : "New deck";

      if (e.PropertyName == nameof(deckEditorViewModel.HasUnsavedChanges))
        tabHeader.UnsavedIndicator = deckEditorViewModel.HasUnsavedChanges;
    }
  }

  private void TabItem_OnClosed(object sender, System.EventArgs e)
  {
    tabItem.OnClosed -= TabItem_OnClosed;
    tabFrame.Navigated -= TabFrame_Navigated;

    if (tabFrame.Content is DeckEditorPage deckEditorPage)
      deckEditorPage.ViewModel.PropertyChanged -= DeckEditorPageViewModel_PropertyChanged;
  }
}
