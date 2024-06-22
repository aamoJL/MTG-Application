using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;

namespace MTGApplication.Features.DeckTesting.Views;
[ObservableObject]
public sealed partial class DeckTestingPage : Page
{
  public DeckTestingPage() => InitializeComponent();

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    ViewModel = new DeckTestingPageViewModel.Factory(App.MTGCardImporter)
      .Build(e.Parameter as DeckTestingDeck);

    ViewModel.NewGameStarted += OnNewGameStarted;
    ViewModel.NewTurnStarted += OnNewTurnStarted;
    ViewModel.StartNewGameCommand.Execute(null);
  }

  public DeckTestingPageViewModel ViewModel { get; set; }

  [ObservableProperty] private Visibility libraryVisibility = Visibility.Collapsed;

  [RelayCommand] private void LibraryVisibilitySwitch() => LibraryVisibility = LibraryVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

  private void OnNewTurnStarted()
  {
    foreach (var child in BattlefieldCanvas.Children)
      if ((child as FrameworkElement).DataContext is DeckTestingMTGCard card)
        card.IsTapped = false;
  }

  private void OnNewGameStarted() => BattlefieldCanvas.Children.Clear();
}
