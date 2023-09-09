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

[ObservableObject]
public sealed partial class MTGCardPreviewControl : UserControl
{
  public partial class CardPreviewProperties : ObservableObject
  {
    [ObservableProperty] private MTGCardViewModel cardViewModel;
    [ObservableProperty] private Vector2 coordinates;
  }

  public MTGCardPreviewControl() => InitializeComponent();

  public CardPreviewProperties PreviewProperties
  {
    get => (CardPreviewProperties)GetValue(PreviewPropertiesProperty);
    set
    {
      SetValue(PreviewPropertiesProperty, value);
      value.PropertyChanged += PreviewProperties_PropertyChanged;
    }
  }

  [ObservableProperty] private Visibility previewVisibility = Visibility.Collapsed;
  [ObservableProperty] private ImageSource previewPlaceholderSource = null;
  [ObservableProperty] private ImageSource previewSource = null;
  [ObservableProperty] private Vector2 previewPosition = Vector2.Zero;

  public static readonly DependencyProperty PreviewPropertiesProperty =
      DependencyProperty.Register(nameof(PreviewProperties), typeof(CardPreviewProperties), typeof(MTGCardPreviewControl), new PropertyMetadata(0));

  private void PreviewProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(PreviewProperties.CardViewModel):
        if (PreviewProperties.CardViewModel != null)
        {
          PreviewSource = new BitmapImage(new(PreviewProperties.CardViewModel.SelectedFaceUri));
        }
        else
        {
          PreviewImage.PlaceholderSource = PreviewImage.Source as ImageSource;
        }
        PreviewVisibility = PreviewProperties.CardViewModel != null ? Visibility.Visible : Visibility.Collapsed;
        break;
      case nameof(PreviewProperties.Coordinates):
        SetPreviewPosition(PreviewProperties.Coordinates);
        break;
    }
  }

  private void SetPreviewPosition(Vector2 coordinates)
  {
    var windowBounds = ActualSize;

    coordinates.X -= ActualOffset.X; // Apply element offset
    coordinates.Y -= ActualOffset.Y;

    var xOffsetFromPointer = (windowBounds.X - coordinates.X) > PreviewImage.ActualWidth ? 50 : -50 - PreviewImage.ActualWidth;
    var yOffsetFromPointer = -100;

    PreviewPosition = new(
      x: (float)Math.Max(Math.Clamp(coordinates.X + xOffsetFromPointer, 0, Math.Max(ActualSize.X - PreviewImage.ActualWidth, 0)), 0),
      y: (float)Math.Max(Math.Clamp(coordinates.Y + yOffsetFromPointer, 0, Math.Max(ActualSize.Y - PreviewImage.ActualHeight, 0)), 0)
    );
  }
}
