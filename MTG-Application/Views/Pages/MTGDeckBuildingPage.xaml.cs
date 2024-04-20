using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.General.Views;
using MTGApplication.Interfaces;
using MTGApplication.Services;
using MTGApplication.Views.Controls;
using MTGApplication.Views.Pages.Tabs;
using MTGApplication.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using static MTGApplication.Services.NotificationService;
using static MTGApplication.Views.Controls.MTGCardPreviewControl;

namespace MTGApplication.Views.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ObservableObject]
public sealed partial class MTGDeckBuildingPage : Page, ISavable, IDialogPresenter
{
  public MTGDeckBuildingPage()
  {
    InitializeComponent();

    Loaded += MTGDeckBuildingPage_Loaded;
    Unloaded += MTGDeckBuildingPage_Unloaded;
  }

  #region Properties
  [ObservableProperty] private bool searchPanelOpen = false;

  public ObservableCollection<TabViewItem> TabViews { get; } = new();
  public CardPreviewProperties CardPreviewProperties { get; } = new() { XMirror = true, Offset = new(175, 100) };
  #endregion

  #region ISavable Implementation
  public bool HasUnsavedChanges
  {
    get => TabViews.FirstOrDefault(x => x.Content is ISavable { HasUnsavedChanges: true }) != null;
    set { return; }
  }

  public async Task<bool> SaveUnsavedChanges()
  {
    foreach (var item in TabViews)
    {
      if (item.Content is ISavable { HasUnsavedChanges: true } savable)
      {
        item.IsSelected = true;
        if (!await savable.SaveUnsavedChanges()) return false;
      }
    }
    return true;
  }
  #endregion

  #region IDialogPresenter implementation
  public DialogService.DialogWrapper DialogWrapper { get; private set; }
  #endregion

  #region Events
  private void MTGDeckBuildingPage_Loaded(object sender, RoutedEventArgs e)
  {
    DialogWrapper = new(XamlRoot);
    OnNotification += Notifications_OnNotification;
    TabViews.Add(CreateNewTab());
  }

  private void MTGDeckBuildingPage_Unloaded(object sender, RoutedEventArgs e)
    => OnNotification -= Notifications_OnNotification;

  private void Notifications_OnNotification(object sender, NotificationEventArgs e)
  {
    if ((XamlRoot)sender == XamlRoot)
    {
      InAppNotification.Background = e.Type switch
      {
        NotificationType.Error => new SolidColorBrush(Color.FromArgb(255, 248, 215, 218)),
        NotificationType.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 243, 205)),
        NotificationType.Success => new SolidColorBrush(Color.FromArgb(255, 212, 237, 218)),
        _ => new SolidColorBrush(Color.FromArgb(255, 204, 229, 255)),
      };
      InAppNotification.RequestedTheme = ElementTheme.Light;
      InAppNotification.Show(e.Text, NotificationDuration);
    }
  }

  private void TabView_AddTabButtonClick(TabView tabView, object args)
  {
    var tab = CreateNewTab();
    TabViews.Add(tab);
    tabView.SelectedItem = tab;
  }

  private async void TabView_TabCloseRequested(TabView tabView, TabViewTabCloseRequestedEventArgs args)
  {
    // Request tab closing from the tab items Content
    if (!await (args.Tab.Content as ITabViewTab)?.TabCloseRequested()) { return; }

    TabViews.Remove(args.Tab);
    args.Tab.Content = null;

    if (TabViews.Count == 0)
    {
      var newTab = CreateNewTab();
      TabViews.Add(newTab);
      tabView.SelectedItem = newTab;
    }
  }
  #endregion

  #region Relay Commands
  /// <summary>
  /// Opens and closes search panel
  /// </summary>
  [RelayCommand]
  public void SwitchSearchPanel() => SearchPanelOpen = !SearchPanelOpen;

  /// <summary>
  /// Changes application color theme
  /// </summary>
  [RelayCommand]
  public void ChangeTheme()
  {
    AppConfig.LocalSettings.AppTheme = AppConfig.LocalSettings.AppTheme == ElementTheme.Dark
      ? ElementTheme.Light : ElementTheme.Dark;
  }

  /// <summary>
  /// Opens card collection window
  /// </summary>
  [RelayCommand]
  public void OpenCollectionsWindow()
  {
    new ThemedWindow
    {
      Content = new MTGCardCollectionPage(),
      Title = "MTG Card Collections"
    }.Activate();
  }
  #endregion

  /// <summary>
  /// Returns new TabViewItem with DeckBuilderTabView as a content
  /// </summary>
  private TabViewItem CreateNewTab()
  {
    var content = new DeckBuilderTabFrame(CardPreviewProperties).Init();

    return new TabViewItem()
    {
      Header = new DeckBuilderTabHeaderControl(content),
      Content = content,
    };
  }
}
