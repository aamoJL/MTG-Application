using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.Interfaces;
using MTGApplication.Services;
using MTGApplication.Views.Pages.Tabs;
using MTGApplication.Views.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI;
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

    Loaded += (s, e) => DialogWrapper = new(XamlRoot);

    NotificationService.OnNotification += Notifications_OnNotification;
  }

  #region Properties
  [ObservableProperty] private bool searchPanelOpen = false;
  [ObservableProperty] private ObservableCollection<DeckBuilderTabView> tabViews = new();

  public CardPreviewProperties CardPreviewProperties { get; } = new() { XMirror = true, Offset = new(175, 100) };
  #endregion

  #region ISavable Implementation
  public bool HasUnsavedChanges
  {
    get => TabViews.FirstOrDefault(x => x.HasUnsavedChanges == true) != null;
    set { return; }
  }

  public async Task<bool> SaveUnsavedChanges()
  {
    foreach (var item in TabViews)
    {
      if (!await item.SaveUnsavedChanges())
      {
        return false;
      }
    }
    return true;
  }
  #endregion

  #region IDialogPresenter implementation
  public DialogService.DialogWrapper DialogWrapper { get; private set; }
  #endregion

  private void Notifications_OnNotification(object sender, NotificationService.NotificationEventArgs e)
  {
    if ((XamlRoot)sender == this.XamlRoot)
    {
      InAppNotification.Background = e.Type switch
      {
        NotificationService.NotificationType.Error => new SolidColorBrush(Color.FromArgb(255, 248, 215, 218)),
        NotificationService.NotificationType.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 243, 205)),
        NotificationService.NotificationType.Success => new SolidColorBrush(Color.FromArgb(255, 212, 237, 218)),
        _ => new SolidColorBrush(Color.FromArgb(255, 204, 229, 255)),
      };
      InAppNotification.RequestedTheme = ElementTheme.Light;
      InAppNotification.Show(e.Text, NotificationService.NotificationDuration);
    }
  }

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

  private void TabView_AddTabButtonClick(TabView tabView, object args)
  {
    var newItem = new DeckBuilderTabView(CardPreviewProperties);
    TabViews.Add(newItem);
    tabView.SelectedItem = newItem;
  }

  private async void TabView_TabCloseRequested(TabView tabView, TabViewTabCloseRequestedEventArgs args)
  {
    // Request tab closing from the tab items Content
    if ((args.Tab.Content as ContentPresenter).Content is DeckBuilderTabView deckBuilder
      && await deckBuilder.TabCloseRequested())
    {
      TabViews.Remove(deckBuilder);
    }
  }

  private void TabView_Loaded(object sender, RoutedEventArgs e) => TabViews.Add(new DeckBuilderTabView(CardPreviewProperties)); // Add default tab

  private void TabView_TabItemsChanged(TabView tabView, IVectorChangedEventArgs args)
  {
    // Disable tab closing if there are only one tab
    if (TabViews.Count == 1)
    {
      TabViews[0].IsClosable = false;
    }
    else if (TabViews.Count > 1)
    {
      TabViews[0].IsClosable = true;
    }
  }
}
