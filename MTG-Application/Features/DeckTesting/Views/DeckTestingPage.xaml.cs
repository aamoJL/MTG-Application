using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.Services;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.Features.DeckTesting.Views.Controls.CardView;
using System.ComponentModel;

namespace MTGApplication.Features.DeckTesting.Views;

public sealed partial class DeckTestingPage : Page, INotifyPropertyChanged
{
  public DeckTestingPage() => InitializeComponent();

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is DeckTestingDeck deck)
    {
      ViewModel = new DeckTestingPageViewModel.Factory(App.MTGCardImporter)
        .Build(deck);

      ViewModel.NewGameStarted += OnNewGameStarted;
      ViewModel.NewTurnStarted += OnNewTurnStarted;
      ViewModel.StartNewGameCommand?.Execute(null);
    }
  }

  public DeckTestingPageViewModel? ViewModel { get; set; }
  public DeckTestingPointerEvents PointerEvents { get; } = new();
  public DeckTestingDragAndDropEvents DragAndDropEvents { get; } = new();

  public Visibility LibraryVisibility
  {
    get => field;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(LibraryVisibility)));
      }
    }
  } = Visibility.Collapsed;

  public event PropertyChangedEventHandler? PropertyChanged;

  [RelayCommand] private void LibraryVisibilitySwitch() => LibraryVisibility = LibraryVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

  private void OnNewTurnStarted()
  {
    // Untap battlefield cards
    foreach (var child in BattlefieldCanvas.Children)
      if (child is DeckTestingBattlefieldCardView cardView)
        cardView.IsTapped = false;
  }

  private void OnNewGameStarted() => BattlefieldCanvas.Children.Clear();
}