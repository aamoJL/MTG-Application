using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MTGApplication.Features.DeckTesting.Services;
using System;
using System.Numerics;

namespace MTGApplication.Features.DeckTesting.Views.Controls;

[ObservableObject]
public sealed partial class DragCardPreview : UserControl
{
  public class DragCardPreviewEventArgs(XamlRoot root) : EventArgs
  {
    public XamlRoot Root { get; } = root;

    public string Uri { get; init; } = null;
    public Vector2? Coordinates { get; init; } = null;
    public Vector2? Offset { get; init; } = null;
    public double? Opacity { get; init; } = null;
    public bool? IsTapped { get; init; } = null;
  }

  public static Vector2 DefaultOffset => new(-(float)(ImageX / 2), -(float)(ImageY / 2));
  public static Vector2 CurrentOffset { get; private set; } = DefaultOffset;
  public static float UndroppableOpacity { get; } = .3f;
  public static float DroppableOpacity { get; } = .8f;
  public static float BattlefieldOpacity { get; } = 1f;
  public static double ImageX { get; } = 215;
  public static double ImageY { get; } = 300;

  public static event EventHandler<DragCardPreviewEventArgs> OnChange;

  public DragCardPreview()
  {
    InitializeComponent();

    Loaded += DragCardPreview_Loaded;
    Unloaded += DragCardPreview_Unloaded;
  }

  [ObservableProperty] private double left = 0;
  [ObservableProperty] private double top = 0;
  [ObservableProperty] private Visibility visibility = Visibility.Collapsed;
  [ObservableProperty] private double opacity = DroppableOpacity;
  [ObservableProperty] private double angle = 0;
  [ObservableProperty] private BitmapImage imageSource = null;

  public double ImageWidth { get; } = ImageX;
  public double ImageHeight { get; } = ImageY;

  private void DragCardPreview_OnChange(object sender, DragCardPreviewEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    if (e.Uri is string uri && ImageSource?.UriSource.OriginalString != uri) ImageSource = new BitmapImage(new Uri(uri));
    if (e.Offset is Vector2 offset) CurrentOffset = offset;
    if (e.Coordinates is Vector2 coordinates)
    {
      var imagePosition = coordinates + CurrentOffset;
      Left = imagePosition.X;
      Top = imagePosition.Y;
    }
    if (e.Opacity is double opacity) Opacity = opacity;
    if (e.IsTapped is bool isTapped) Angle = isTapped ? 90 : 0;
  }

  private void DragCardPreview_Loaded(object sender, RoutedEventArgs e)
  {
    OnChange += DragCardPreview_OnChange;
    DeckTestingCardDrag.Started += CardDragArgs_Started;
    DeckTestingCardDrag.Ended += CardDragArgs_Ended;
    XamlRoot.Content.PointerMoved += Root_PointerMoved;
  }

  private void DragCardPreview_Unloaded(object sender, RoutedEventArgs e)
  {
    OnChange -= DragCardPreview_OnChange;
    DeckTestingCardDrag.Started -= CardDragArgs_Started;
    DeckTestingCardDrag.Ended -= CardDragArgs_Ended;

    if (XamlRoot?.Content != null)
      XamlRoot.Content.PointerMoved -= Root_PointerMoved;
  }

  private void Root_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!DeckTestingCardDrag.IsDragging) return;

    var pointerPosition = e.GetCurrentPoint(null).Position;

    Change(this, new(XamlRoot)
    {
      Coordinates = new((float)pointerPosition.X, (float)pointerPosition.Y)
    });
  }

  private void CardDragArgs_Started() => Visibility = Visibility.Visible;

  private void CardDragArgs_Ended() => Visibility = Visibility.Collapsed;

  public static void Change(object sender, DragCardPreviewEventArgs args) => OnChange?.Invoke(sender, args);
}
