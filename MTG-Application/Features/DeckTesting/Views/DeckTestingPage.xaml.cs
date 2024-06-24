using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.Services;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.Features.DeckTesting.Views.Controls;

namespace MTGApplication.Features.DeckTesting.Views;
[ObservableObject]
public sealed partial class DeckTestingPage : Page
{
  public DeckTestingPage()
  {
    InitializeComponent();

    Loaded += DeckTestingPage_Loaded;
    Unloaded += DeckTestingPage_Unloaded;
  }

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

  private void DeckTestingPage_Loaded(object sender, RoutedEventArgs e)
  {
    if (XamlRoot.Content is UIElement root)
    {
      root.PointerMoved += Root_PointerMoved;
      root.PointerReleased += Root_PointerReleased;
    }

    CardDragArgs.Ended += CardDragArgs_Ended;
  }

  private void DeckTestingPage_Unloaded(object sender, RoutedEventArgs e)
  {
    if (XamlRoot.Content is UIElement root)
    {
      root.PointerMoved -= Root_PointerMoved;
      root.PointerReleased -= Root_PointerReleased;
    }

    CardDragArgs.Ended -= CardDragArgs_Ended;
  }

  private void Root_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (CardDragArgs.IsDragging) CardDragArgs.Cancel();
  }

  private void Root_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (CardDragArgs.IsDragging)
    {
      if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed || !e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
        CardDragArgs.Cancel();
      else
      {
        var pointerPosition = e.GetCurrentPoint(null).Position;

        DragCardPreview.Change(this, new(XamlRoot)
        {
          Coordinates = new((float)pointerPosition.X, (float)pointerPosition.Y),
        });
      }
    }
  }

  private void CardDragArgs_Ended()
  {
    DragCardPreview.Change(this, new(XamlRoot)
    {
      Uri = null,
    });
  }
}
