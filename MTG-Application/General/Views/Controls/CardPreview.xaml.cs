using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Numerics;

namespace MTGApplication.General.Views.Controls;

public sealed partial class CardPreview : UserControl, INotifyPropertyChanged
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

    Loaded += (_, _) => { OnChange += CardPreview_OnChange; };
    Unloaded += (_, _) => { OnChange -= CardPreview_OnChange; };
  }

  public double Left
  {
    get => field;
    set
    {
      field = value;
      PropertyChanged?.Invoke(this, new(nameof(Left)));
    }
  } = 0;
  public double Top
  {
    get => field;
    set
    {
      field = value;
      PropertyChanged?.Invoke(this, new(nameof(Top)));
    }
  } = 0;
  public BitmapImage ImageSource
  {
    get => field;
    set
    {
      field = value;
      PropertyChanged?.Invoke(this, new(nameof(ImageSource)));
    }
  }
  public BitmapImage PlaceholderSource
  {
    get => field;
    set
    {
      field = value;
      PropertyChanged?.Invoke(this, new(nameof(PlaceholderSource)));
    }
  }

  public double ImageWidth { get; } = ImageX;
  public double ImageHeight { get; } = ImageY;

  public event PropertyChangedEventHandler PropertyChanged;

  private void CardPreview_OnChange(object sender, CardPreviewEventArgs e)
  {
    if (e.Root != XamlRoot)
      return;

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
      position.X += (offset.X * -2); // Flip X offset
    if (maxY > windowBounds.Y || minY < 0)
      position.Y += (offset.Y * -2); // Flip Y offset

    // Clamp to window bounds
    return new(
      x: (float)Math.Max(Math.Clamp(position.X, 0, Math.Max(windowBounds.X - ImageWidth, 0)), 0),
      y: (float)Math.Max(Math.Clamp(position.Y, 0, Math.Max(windowBounds.Y - ImageHeight, 0)), 0)
    );
  }

  public static void Change(object sender, CardPreviewEventArgs args) => OnChange?.Invoke(sender, args);
}
