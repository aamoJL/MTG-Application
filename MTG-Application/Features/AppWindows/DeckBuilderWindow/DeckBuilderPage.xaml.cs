using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Views.Controls;
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

  public ObservableCollection<CustomTabViewItem> TabViewItems { get; } = new();

  [ObservableProperty] private bool isSearchPaneOpen = false;

  [RelayCommand] public void SwitchSearchPanel() => IsSearchPaneOpen = !IsSearchPaneOpen;

  [RelayCommand] public void OpenCardCollectionWindow() => new OpenMTGCardCollectionWindow().Execute();

  [RelayCommand]
  public void SwitchWindowTheme()
  {
    new ChangeWindowTheme(AppConfig.LocalSettings.AppTheme == ElementTheme.Dark
      ? ElementTheme.Light : ElementTheme.Dark).Execute();
  }

  private async void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
  {
    // Request tab closing from the tab items Content
    if (args.Tab.Content is ITabViewTab tabContent && !await tabContent.TabCloseRequested()) return;

    if(args.Item is CustomTabViewItem tabItem)
    {
      tabItem.Close();
      TabViewItems.Remove(tabItem);
    }

    args.Tab.Content = null;

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
