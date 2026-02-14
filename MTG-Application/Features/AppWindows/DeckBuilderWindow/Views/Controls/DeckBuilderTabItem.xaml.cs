using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.DeckEditor.Views;
using MTGApplication.Features.DeckSelection.Views;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.Views.Controls;

public sealed partial class DeckBuilderTabItem : TabViewItem
{
  public DeckBuilderTabItem()
  {
    InitializeComponent();

    DataContextChanged += DeckBuilderTabItem_DataContextChanged;
    CloseRequested += DeckBuilderTabItem_CloseRequested;
    ContentFrame.Navigated += Frame_Navigated;
  }

  private void DeckBuilderTabItem_DataContextChanged(FrameworkElement _, DataContextChangedEventArgs __)
  {
    ContentFrame.BackStack.Clear();

    if (DataContext is not DeckBuilderTabViewModel)
      return;

    ContentFrame.Navigate(typeof(DeckSelectionPage), null, new SuppressNavigationTransitionInfo());
  }

  private void Frame_Navigated(object _, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
  {
    if (e.Content is DeckSelectionPage selectionPage)
    {
      selectionPage.OnDeckSelected = (selectedDeck) =>
      {
        ContentFrame.Navigate(typeof(DeckEditorPage), selectedDeck.Name, new SuppressNavigationTransitionInfo());
      };
    }
    else if (e.Content is DeckEditorPage editorPage)
      (DataContext as DeckBuilderTabViewModel)?.ChangeViewModelCommand.Execute(editorPage.ViewModel);
  }

  private async void DeckBuilderTabItem_CloseRequested(TabViewItem _, TabViewTabCloseRequestedEventArgs __)
  {
    if (DataContext is not DeckBuilderTabViewModel viewmodel)
      return;

    await viewmodel.TryCloseCommand.ExecuteAsync(null);
  }
}

