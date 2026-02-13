using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.DeckEditor.Editor.UseCases;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.Features.DeckEditor.Views;
using MTGApplication.Features.DeckSelection.Views;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;

public sealed partial class DeckBuilderTabItem : TabViewItem
{
  private static readonly string DefaultDeckSelectionTabHeaderText = "New tab";
  private static readonly string DefaultNewDeckTabHeaderText = "New deck";

  public DeckBuilderTabItem()
  {
    InitializeComponent();

    Header = new TabHeader() { Text = DefaultDeckSelectionTabHeaderText };

    CloseRequested += DeckBuilderTabItem_CloseRequested;
    ContentFrame.Navigated += Frame_Navigated;

    ContentFrame.Navigate(typeof(DeckSelectionPage));
  }

  public TabHeader? TabHeader => Header as TabHeader;
  public Action<DeckBuilderTabItem>? OnClose;

  public async Task RequestClose(SaveStatus.ConfirmArgs args)
  {
    // TODO: change to datacontext
    if (ContentFrame.Content is DeckEditorPage editorPage && editorPage.ViewModel.HasUnsavedChanges)
    {
      IsSelected = true;
      await new ConfirmUnsavedChanges(editorPage.ViewModel).Execute(args);
    }
  }

  private void Frame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
  {
    switch (e.Content)
    {
      case DeckSelectionPage selectionPage:
        selectionPage.OnDeckSelected = (selectedDeck) =>
        {
          ContentFrame.Navigate(typeof(DeckEditorPage), selectedDeck.Name, new SuppressNavigationTransitionInfo());
        };
        break;
      case DeckEditorPage deckEditorPage:
        TabHeader?.Text = !string.IsNullOrEmpty(deckEditorPage.ViewModel.Name) ? deckEditorPage.ViewModel.Name : DefaultNewDeckTabHeaderText;
        deckEditorPage.ViewModel.PropertyChanged += DeckEditorPageViewModel_PropertyChanged;
        break;
    }
  }

  private void DeckEditorPageViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is not DeckEditorViewModel deckEditorViewModel) return;

    switch (e.PropertyName)
    {
      case nameof(deckEditorViewModel.Name):
        TabHeader?.Text = !string.IsNullOrEmpty(deckEditorViewModel.Name) ? deckEditorViewModel.Name : DefaultNewDeckTabHeaderText;
        break;
      case nameof(deckEditorViewModel.HasUnsavedChanges):
        TabHeader?.UnsavedIndicator = deckEditorViewModel.HasUnsavedChanges;
        break;
    }
  }

  private async void DeckBuilderTabItem_CloseRequested(TabViewItem sender, TabViewTabCloseRequestedEventArgs args)
  {
    var saveArgs = new SaveStatus.ConfirmArgs();

    await RequestClose(saveArgs);

    if (saveArgs.Cancelled)
      return;

    (ContentFrame?.Content as DeckEditorPage)?.ViewModel.PropertyChanged -= DeckEditorPageViewModel_PropertyChanged;
    ContentFrame?.Navigated -= Frame_Navigated;
    CloseRequested -= DeckBuilderTabItem_CloseRequested;

    OnClose?.Invoke(this);
  }
}

