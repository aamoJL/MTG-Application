using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MTGApplication.ViewModels;
using System;
using System.ComponentModel;
using System.Numerics;

namespace MTGApplication.Views.Controls;

/// <summary>
/// Control to show preview image of a <see cref="MTGCardViewModel"/>
/// </summary>
[ObservableObject]
public sealed partial class MTGCardPreviewControl : UserControl
{
  public MTGCardPreviewControl() => InitializeComponent();

  #region Properties
  [ObservableProperty] private Visibility previewVisibility = Visibility.Collapsed;
  [ObservableProperty] private ImageSource previewPlaceholderSource = null;
  [ObservableProperty] private ImageSource previewSource = null;
  [ObservableProperty] private Vector2 previewPosition = Vector2.Zero;

  public CardPreviewProperties PreviewProperties
  {
    get => (CardPreviewProperties)GetValue(PreviewPropertiesProperty);
    set
    {
      SetValue(PreviewPropertiesProperty, value);
      InitPreviewImage();
    }
  }
  #endregion

  #region Dependency Properties
  public static readonly DependencyProperty PreviewPropertiesProperty =
      DependencyProperty.Register(nameof(PreviewProperties), typeof(CardPreviewProperties), typeof(MTGCardPreviewControl), new PropertyMetadata(0));
  #endregion

  #region OnPropertyChanged events
  private void PreviewProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(PreviewProperties.CardViewModel):
        if (PreviewProperties.CardViewModel == null)
          HidePreviewImage();
        else
          ShowPreviewImage();
        break;
      case nameof(PreviewProperties.Coordinates):
        SetPreviewPosition();
        break;
    }
  }
  #endregion

  /// <summary>
  /// Initializes preview image
  /// </summary>
  private void InitPreviewImage()
  {
    if (PreviewProperties != null)
    {
      PreviewProperties.PropertyChanged += PreviewProperties_PropertyChanged;
    }

    if (PreviewProperties?.CardViewModel != null)
    {
      ShowPreviewImage();
      SetPreviewPosition();
    }
    else
    {
      HidePreviewImage();
    }
  }

  /// <summary>
  /// Shows preview image
  /// </summary>
  private void ShowPreviewImage()
  {
    PreviewSource = new BitmapImage(new(PreviewProperties.CardViewModel?.SelectedFaceUri));
    PreviewVisibility = Visibility.Visible;
  }

  /// <summary>
  /// Hides preview image
  /// </summary>
  private void HidePreviewImage()
  {
    PreviewVisibility = Visibility.Collapsed;
    PreviewPlaceholderSource = PreviewSource; // Reduces flickering
    PreviewSource = null;
  }

  /// <summary>
  /// Calculates and sets the preview image's position from the preview properties
  /// </summary>
  private void SetPreviewPosition()
  {
    var windowBounds = ActualSize;
    var position = PreviewProperties.Coordinates + PreviewProperties.Offset
      - new Vector2(PreviewProperties.Width / 2, PreviewProperties.Height / 2); // Image pivot is upper left corner

    var minX = position.X;
    var maxX = position.X + PreviewProperties.Width;
    var minY = position.Y;
    var maxY = position.Y + PreviewProperties.Height;

    // Mirror offset
    if (PreviewProperties.XMirror && (maxX > windowBounds.X || minX < 0))
    {
      position.X += (PreviewProperties.Offset.X * -2); // Flip X offset
    }
    if (PreviewProperties.YMirror && (maxY > windowBounds.Y || minY < 0))
    {
      position.Y += (PreviewProperties.Offset.Y * -2); // Flip Y offset
    }

    // Clamp to window bounds
    PreviewPosition = new(
      x: (float)Math.Max(Math.Clamp(position.X, 0, Math.Max(windowBounds.X - PreviewProperties.Width, 0)), 0),
      y: (float)Math.Max(Math.Clamp(position.Y, 0, Math.Max(windowBounds.Y - PreviewProperties.Height, 0)), 0)
    );
  }
}

public sealed partial class MTGCardPreviewControl
{
  public partial class CardPreviewProperties : ObservableObject
  {
    [ObservableProperty] private MTGCardViewModel cardViewModel;
    [ObservableProperty] private Vector2 coordinates;

    /// <summary>
    /// Position offset from the coordinates
    /// </summary>
    public Vector2 Offset { get; init; } = Vector2.Zero;
    /// <summary>
    /// Mirror the X-axis offset, instead of clamping, if the image does not fit into the window
    /// </summary>
    public bool XMirror { get; init; } = false;
    /// <summary>
    /// Mirror the Y-axis offset, instead of clamping, if the image does not fit into the window
    /// </summary>
    public bool YMirror { get; init; } = false;
    /// <summary>
    /// Image height
    /// </summary>
    public int Height { get; init; } = 350;
    /// <summary>
    /// Image width
    /// </summary>
    public int Width { get; init; } = 251;
  }
}
