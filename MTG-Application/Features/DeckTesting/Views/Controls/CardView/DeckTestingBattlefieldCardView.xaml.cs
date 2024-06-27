using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.Services;
using MTGApplication.General.Views.Extensions;
using System;
using System.Linq;
using Windows.Foundation;

namespace MTGApplication.Features.DeckTesting.Views.Controls.CardView;
public sealed partial class DeckTestingBattlefieldCardView : DeckTestingCardViewBase
{
  public enum PointerButton { None, Left, Middle }

  public DeckTestingBattlefieldCardView() => InitializeComponent();

  public int PlusCounters
  {
    get => plusCounters;
    set => SetProperty(ref plusCounters, Math.Max(0, value));
  }
  public int CountCounters
  {
    get => countCounters;
    set => SetProperty(ref countCounters, Math.Max(1, value));
  }

  [ObservableProperty] private bool isTapped = false;
  [ObservableProperty] private Visibility plusCounterVisibility = Visibility.Collapsed;
  [ObservableProperty] private Visibility countCounterVisibility = Visibility.Collapsed;

  public double CardWidth => DragCardPreview.ImageX;
  public double CardHeight => DragCardPreview.ImageY;

  private int plusCounters = 0;
  private int countCounters = 1;
  private PointerButton lastButtonPress = PointerButton.None; // used for click actions using press/release/move events

  protected override void OnPointerPressed(PointerRoutedEventArgs e)
  {
    var properties = e.GetCurrentPoint(null).Properties;

    if (properties.IsLeftButtonPressed)
      lastButtonPress = PointerButton.Left;
    else if (properties.IsMiddleButtonPressed)
      lastButtonPress = PointerButton.Middle;

    XamlRoot.Content.PointerMoved += Root_PointerMoved;
  }

  protected override void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    base.Root_PointerMoved(sender, e);

    var properties = e.GetCurrentPoint(null).Properties;

    if (properties.IsLeftButtonPressed && !DeckTestingCardDrag.IsDragging)
    {
      var elementPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
      var canvas = this.FindParentByType<Canvas>();
      var pointerCanvasPosition = e.GetCurrentPoint(canvas).Position;
      var pointerWindowPosition = e.GetCurrentPoint(null).Position;
      var offset = new Point(
        x: elementPosition.X - pointerCanvasPosition.X,
        y: elementPosition.Y - pointerCanvasPosition.Y);

      XamlRoot.Content.PointerReleased += Root_PointerReleased;

      Visibility = Visibility.Collapsed;

      DragCardPreview.Change(this, new(XamlRoot)
      {

        Coordinates = new(x: (float)pointerWindowPosition.X, y: (float)pointerWindowPosition.Y),
        Offset = new((float)offset.X, (float)offset.Y),
        Opacity = DragCardPreview.BattlefieldOpacity,
        IsTapped = IsTapped
      });

      DeckTestingCardDrag.Completed += OnDragCompleted;
      DeckTestingCardDrag.Ended += OnDragEnded;

      DeckTestingCardDrag.Start(Model);
    }
  }

  protected override void OnPointerReleased(PointerRoutedEventArgs e)
  {
    base.OnPointerReleased(e);

    XamlRoot.Content.PointerMoved -= Root_PointerMoved;

    switch (lastButtonPress)
    {
      case PointerButton.Left:
        IsTapped = !IsTapped; break;
      case PointerButton.Middle:
        PlusCounterVisibility = PlusCounterVisibility == Visibility.Visible
          ? Visibility.Collapsed : Visibility.Visible; break;
    }

    lastButtonPress = PointerButton.None;
  }

  protected override void OnPointerMoved(PointerRoutedEventArgs e)
  {
    base.OnPointerMoved(e);

    lastButtonPress = PointerButton.None;
  }

  private void OnDragCompleted(DeckTestingMTGCard item)
  {
    if (this.FindParentByType<Canvas>() is not Canvas canvas)
      return;

    canvas?.Children.Remove(canvas.Children
      .FirstOrDefault(c => (c as DeckTestingBattlefieldCardView).Model == item));
  }

  private void OnDragEnded()
  {
    DragCardPreview.Change(this, new(XamlRoot)
    {
      IsTapped = false,
    });

    XamlRoot.Content.PointerMoved -= Root_PointerMoved;
    XamlRoot.Content.PointerReleased -= Root_PointerReleased;
    DeckTestingCardDrag.Completed -= OnDragCompleted;
    DeckTestingCardDrag.Ended -= OnDragEnded;

    Visibility = Visibility.Visible;
  }

  private void PlusCounterFlyoutButton_Click(object sender, RoutedEventArgs e)
    => PlusCounterVisibility = PlusCounterVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

  private void CountCounterFlyoutButton_Click(object sender, RoutedEventArgs e)
    => CountCounterVisibility = CountCounterVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

  private void PlusCounter_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.MouseWheelDelta > 0) PlusCounters++;
    else PlusCounters--;
  }

  private void CountCounter_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.MouseWheelDelta > 0) CountCounters++;
    else CountCounters--;
  }
}
