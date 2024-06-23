using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Numerics;

namespace MTGApplication.General.Views.Controls;

[ObservableObject]
public sealed partial class CardPreview : UserControl
{
  public class CardPreviewEventArgs(XamlRoot root) : EventArgs
  {
    public XamlRoot Root { get; } = root;
    
    public string Uri { get; init; } = string.Empty;
    public Vector2 Coordinates { get; init; } = Vector2.Zero;
    public Vector2 OffsetOverride { get; init; } = ImageOffset;
  }

  public static double ImageX { get; } = 251;
  public static double ImageY { get; } = 350;
  public static Vector2 ImageOffset { get; } = new(175, 100);

  public static event EventHandler<CardPreviewEventArgs> OnChange;

  public CardPreview()
  {
    InitializeComponent();

    Loaded += (s, e) => { OnChange += CardPreview_OnChange; };
    Unloaded += (s, e) => { OnChange -= CardPreview_OnChange; };
  }

  [ObservableProperty] private double left = 0;
  [ObservableProperty] private double top = 0;
  [ObservableProperty] private Visibility visibility = Visibility.Collapsed;
  [ObservableProperty] private BitmapImage imageSource = null;
  [ObservableProperty] private BitmapImage placeholderSource = null;

  public double ImageWidth { get; } = ImageX;
  public double ImageHeight { get; } = ImageY;

  private void CardPreview_OnChange(object sender, CardPreviewEventArgs e)
  {
    if (e.Root != XamlRoot) return;

    if (string.IsNullOrEmpty(e.Uri))
    {
      Visibility = Visibility.Collapsed;
      PlaceholderSource = ImageSource; // Prevents flickering
    }
    else
    {
      Visibility = Visibility.Visible;

      if (ImageSource?.UriSource.OriginalString != e.Uri)
        ImageSource = new BitmapImage(new Uri(e.Uri));

      var imagePosition = GetPreviewPosition(e.Coordinates, e.OffsetOverride);
      Left = imagePosition.X;
      Top = imagePosition.Y;
    }
  }

  private Vector2 GetPreviewPosition(Vector2 coordinates, Vector2 offset)
  {
    var windowBounds = ActualSize;
    var position = coordinates + offset
      - new Vector2((float)ImageWidth / 2, (float)ImageHeight / 2); // Image pivot is upper left corner

    var minX = position.X;
    var maxX = position.X + ImageWidth;
    var minY = position.Y;
    var maxY = position.Y + ImageHeight;

    // Mirror offset if the image is over the boundaries
    if (maxX > windowBounds.X || minX < 0)
    {
      position.X += (offset.X * -2); // Flip X offset
    }
    if (maxY > windowBounds.Y || minY < 0)
    {
      position.Y += (offset.Y * -2); // Flip Y offset
    }

    // Clamp to window bounds
    return new(
      x: (float)Math.Max(Math.Clamp(position.X, 0, Math.Max(windowBounds.X - ImageWidth, 0)), 0),
      y: (float)Math.Max(Math.Clamp(position.Y, 0, Math.Max(windowBounds.Y - ImageHeight, 0)), 0)
    );
  }

  public static void Change(object sender, CardPreviewEventArgs args) => OnChange?.Invoke(sender, args);
}
