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

    public string Uri { get; init; } = string.Empty;
    public Vector2? Coordinates { get; init; } = null;
    public Vector2 OffsetOverride { get; init; } = new(-(float)(ImageX / 2), -(float)(ImageY / 2));
  }

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
  [ObservableProperty] private double opacity = .5f;
  [ObservableProperty] private BitmapImage imageSource = null;

  public double ImageWidth { get; } = ImageX;
  public double ImageHeight { get; } = ImageY;

  private void DragCardPreview_OnChange(object sender, DragCardPreviewEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    if (e.Uri == null)
      Visibility = Visibility.Collapsed;
    else
    {
      if (e.Uri != string.Empty && ImageSource?.UriSource.OriginalString != e.Uri)
        ImageSource = new BitmapImage(new Uri(e.Uri));

      if (e.Coordinates is Vector2 coordinates)
      {
        var imagePosition = coordinates + e.OffsetOverride;

        Left = imagePosition.X;
        Top = imagePosition.Y;
        Visibility = Visibility.Visible;
      }
    }
  }

  private void DragCardPreview_Loaded(object sender, RoutedEventArgs e) => OnChange += DragCardPreview_OnChange;

  private void DragCardPreview_Unloaded(object sender, RoutedEventArgs e) => OnChange -= DragCardPreview_OnChange;

  public static void Change(object sender, DragCardPreviewEventArgs args) => OnChange?.Invoke(sender, args);
}
