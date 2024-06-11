using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow;
[ObservableObject]
public sealed partial class DeckBuilderPage : Page
{
  public DeckBuilderPage() => InitializeComponent();

  public ObservableCollection<DeckSelectorAndEditorTabViewItem> TabViewItems { get; } = [new CreateNewDeckViewTabItem().Execute()];

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
    if (!await (args.Item as DeckSelectorAndEditorTabViewItem).RequestClosure())
      return;

    TabViewItems.Remove(args.Item as DeckSelectorAndEditorTabViewItem);

    // Create new tab if there are no tabs
    if (TabViewItems.Count == 0)
      AddNewTab(sender);
  }

  private void TabView_AddTabButtonClick(TabView sender, object args) => AddNewTab(sender);

  private void AddNewTab(TabView tabView)
  {
    var newTab = new CreateNewDeckViewTabItem().Execute();
    TabViewItems.Add(newTab);
    tabView.SelectedItem = newTab;
  }
}