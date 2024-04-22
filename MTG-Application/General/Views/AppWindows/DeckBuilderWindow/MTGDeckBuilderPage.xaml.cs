using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace MTGApplication.General.Views;
[ObservableObject]
public sealed partial class MTGDeckBuilderPage : Page
{
  public MTGDeckBuilderPage()
  {
    InitializeComponent();

    TabViewItems.Add(new CreateNewDeckViewTabItemUseCase().Execute());
  }

  public ObservableCollection<CustomTabViewItem> TabViewItems { get; } = new();

  [ObservableProperty] private bool isSearchPaneOpen = false;

  [RelayCommand] public void SwitchSearchPanel() => IsSearchPaneOpen = !IsSearchPaneOpen;

  [RelayCommand] public void OpenCardCollectionWindow() => new OpenMTGCardCollectionWindowUseCase().Execute();

  [RelayCommand]
  public void SwitchWindowTheme()
    => new ChangeWindowThemeUseCase(AppConfig.LocalSettings.AppTheme == ElementTheme.Dark
      ? ElementTheme.Light : ElementTheme.Dark).Execute();

  private async void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
  {
    // Request tab closing from the tab items Content
    if (args.Tab.Content is ITabViewTab tabContent && !await tabContent.TabCloseRequested()) return;

    TabViewItems.Remove(args.Item as CustomTabViewItem);
    args.Tab.Content = null;

    if (TabViewItems.Count == 0)
    {
      var newTab = new CreateNewDeckViewTabItemUseCase().Execute();
      TabViewItems.Add(newTab);
      sender.SelectedItem = newTab;
    }
  }

  private void TabView_AddTabButtonClick(TabView sender, object args)
  {
    var newTab = new CreateNewDeckViewTabItemUseCase().Execute();
    TabViewItems.Add(newTab);
    sender.SelectedItem = newTab;
  }
}
