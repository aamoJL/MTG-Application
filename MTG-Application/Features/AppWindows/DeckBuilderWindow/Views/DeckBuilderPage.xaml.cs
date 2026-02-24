using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;
using MTGApplication.General.Views.AppWindows;
using MTGApplication.General.Views.AppWindows.UseCases;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.Views;

public sealed partial class DeckBuilderPage : Page, INotifyPropertyChanged
{
  public DeckBuilderPage()
  {
    InitializeComponent();

    WindowClosing.Closing += WindowClosing_Closing;
    WindowClosing.Closed += WindowClosing_Closed;
  }

  public ObservableCollection<DeckBuilderTabViewModel> DeckBuilderTabs => field ??= [BuildTab()];
  public DeckBuilderTabViewModel? SelectedTab
  {
    get => field ??= DeckBuilderTabs.FirstOrDefault();
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(SelectedTab)));
      }
    }
  }
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

  [RelayCommand]
  private void SwitchSearchPanel() => IsSearchPaneOpen = !IsSearchPaneOpen;

  [RelayCommand]
  private void AddDeckBuilderTab()
  {
    var newTab = BuildTab();
    DeckBuilderTabs.Add(newTab);
    SelectedTab = newTab;
  }

  [RelayCommand]
  private void OpenCardCollectionWindow() => new OpenMTGCardCollectionWindow().Execute();

  [RelayCommand]
  private void SwitchWindowTheme() => new ChangeWindowTheme().Execute(AppConfig.LocalSettings.AppTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark);

  private void WindowClosing_Closing(object? sender, WindowClosing.ClosingEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    foreach (var tabItem in DeckBuilderTabs)
      e.Tasks.Add(tabItem.RequestCloseCommand.ExecuteAsync);
  }

  private void WindowClosing_Closed(object? sender, WindowClosing.ClosedEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    foreach (var item in DeckBuilderTabs.ToArray())
      RemoveTab(item);

    WindowClosing.Closing -= WindowClosing_Closing;
    WindowClosing.Closed -= WindowClosing_Closed;
  }

  private void RemoveTab(DeckBuilderTabViewModel tabItem)
  {
    // Workaround - removing tabview will throw an exception if list item has focus on the tab.
    Focus(FocusState.Programmatic);

    DeckBuilderTabs.Remove(tabItem);
    tabItem.ChangeViewModelCommand.Execute(null);
  }

  private DeckBuilderTabViewModel BuildTab()
  {
    return new DeckBuilderTabViewModel()
    {
      OnRequestSelection = tab => SelectedTab = tab,
      OnClose = tab =>
      {
        RemoveTab(tab);

        // Create new tab if there are no tabs
        if (DeckBuilderTabs.Count == 0)
          AddDeckBuilderTab();
      }
    };
  }
}