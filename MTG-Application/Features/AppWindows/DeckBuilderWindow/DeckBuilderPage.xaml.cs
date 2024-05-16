using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow;
[ObservableObject]
public sealed partial class DeckBuilderPage : Page
{
  public DeckBuilderPage()
  {
    InitializeComponent();

    TabViewItems.Add(new CreateNewDeckViewTabItem().Execute());
  }

  public ObservableCollection<DeckEditorTabViewItem> TabViewItems { get; } = new();

  [ObservableProperty] private bool isSearchPaneOpen = false;

  [RelayCommand] public void SwitchSearchPanel() => IsSearchPaneOpen = !IsSearchPaneOpen;

  [RelayCommand] public void OpenCardCollectionWindow() => new OpenMTGCardCollectionWindow().Execute();

  [RelayCommand]
  public void SwitchWindowTheme()
  {
    new ChangeWindowTheme(AppConfig.LocalSettings.AppTheme == ElementTheme.Dark
      ? ElementTheme.Light : ElementTheme.Dark).Execute();
  }

  private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
  {
    if (args.Item is not DeckEditorTabViewItem tabItem) return;

    // TODO: save unsaved changes
    TabViewItems.Remove(tabItem);

    if (TabViewItems.Count == 0)
    {
      var newTab = new CreateNewDeckViewTabItem().Execute();
      TabViewItems.Add(newTab);
      sender.SelectedItem = newTab;
    }
  }

  private void TabView_AddTabButtonClick(TabView sender, object args)
  {
    var newTab = new CreateNewDeckViewTabItem().Execute();
    TabViewItems.Add(newTab);
    sender.SelectedItem = newTab;
  }
}
