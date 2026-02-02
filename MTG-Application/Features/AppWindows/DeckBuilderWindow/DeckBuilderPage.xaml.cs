using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;
using MTGApplication.General.Views.AppWindows;
using MTGApplication.General.Views.AppWindows.UseCases;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow;

public sealed partial class DeckBuilderPage : Page, INotifyPropertyChanged
{
  public DeckBuilderPage()
  {
    InitializeComponent();

    WindowClosing.Closing += WindowClosing_Closing;
    WindowClosing.Closed += WindowClosing_Closed;

    AddDeckBuilderTab();
  }

  public ObservableCollection<DeckBuilderTabItem> DeckBuilderTabs = [];
  public bool IsSearchPaneOpen
  {
    get => field;
    set
    {
      field = value;
      PropertyChanged?.Invoke(this, new(nameof(IsSearchPaneOpen)));
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  public Action OpenCardCollectionWindow_UC { private get => field ??= new OpenMTGCardCollectionWindow().Execute; set; }
  public Action<ElementTheme> ChangeAppTheme_UC { private get => field ??= new ChangeWindowTheme().Execute; set; }

  [RelayCommand]
  private void SwitchSearchPanel() => IsSearchPaneOpen = !IsSearchPaneOpen;

  [RelayCommand]
  private void AddDeckBuilderTab()
  {
    var newTab = new DeckBuilderTabItem()
    {
      OnClose = RemoveTab
    };
    DeckBuilderTabs.Add(newTab);
    DeckBuilderTabView.SelectedItem = newTab;
  }

  [RelayCommand]
  private void OpenCardCollectionWindow() => OpenCardCollectionWindow_UC();

  [RelayCommand]
  private void SwitchWindowTheme() => ChangeAppTheme_UC(AppConfig.LocalSettings.AppTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark);

  private void WindowClosing_Closing(object? sender, WindowClosing.ClosingEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    foreach (var tabItem in DeckBuilderTabs)
      e.Tasks.Add(tabItem.RequestClose);
  }

  private void WindowClosing_Closed(object? sender, WindowClosing.ClosedEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    WindowClosing.Closing -= WindowClosing_Closing;
    WindowClosing.Closed -= WindowClosing_Closed;
  }

  private void RemoveTab(DeckBuilderTabItem tabItem)
  {
    // Workaround - removing tabview will throw an exception if list item has focus on the tab.
    Focus(FocusState.Programmatic);

    DeckBuilderTabs.Remove(tabItem);

    // Create new tab if there are no tabs
    if (DeckBuilderTabs.Count == 0)
      AddDeckBuilderTab();
  }
}