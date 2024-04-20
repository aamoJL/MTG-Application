using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.CardDeck;
using System.Collections.ObjectModel;

namespace MTGApplication.General.Views;
[ObservableObject]
public sealed partial class MTGDeckBuilderPage : Page
{
  public MTGDeckBuilderPage()
  {
    InitializeComponent();

    TabViewItems.Add(CreateNewTab());
  }

  public ObservableCollection<TabViewItem> TabViewItems { get; } = new();

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

    TabViewItems.Remove(args.Tab);
    args.Tab.Content = null;

    if (TabViewItems.Count == 0)
    {
      var newTab = CreateNewTab();
      TabViewItems.Add(newTab);
      sender.SelectedItem = newTab;
    }
  }

  private void TabView_AddTabButtonClick(TabView sender, object args)
  {
    var newTab = CreateNewTab();
    TabViewItems.Add(newTab);
    sender.SelectedItem = newTab;
  }

  private TabViewItem CreateNewTab()
  {
    var tabFrame = new Frame();

    tabFrame.Content = new MTGDeckSelectorView()
    {
      DeckSelected = new RelayCommand<string>((string selectedDeck) =>
      {
        tabFrame.Navigate(typeof(Page), selectedDeck ?? "", new SuppressNavigationTransitionInfo());
      }),
    };

    return new TabViewItem()
    {
      Header = new TextBlock() { Text = "New tab" },
      Content = tabFrame,
    };
  }
}
