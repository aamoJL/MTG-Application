using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.UseCases;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow;

public sealed partial class DeckBuilderPage : Page, INotifyPropertyChanged
{
  public DeckBuilderPage() => InitializeComponent();

  public ObservableCollection<DeckSelectionAndEditorTabViewItem> TabViewItems { get; } = [new CreateNewDeckViewTabItem().Execute()];

  public bool IsSearchPaneOpen { get => field; set { field = value; PropertyChanged?.Invoke(this, new(nameof(IsSearchPaneOpen))); } } = false;

  public event PropertyChangedEventHandler? PropertyChanged;

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
    if (args.Item is not DeckSelectionAndEditorTabViewItem item) return;

    var unsavedArgs = new ISavable.ConfirmArgs();

    await item.RequestClosure(unsavedArgs);

    if (unsavedArgs.Cancelled)
      return;

    TabViewItems.Remove(item);

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