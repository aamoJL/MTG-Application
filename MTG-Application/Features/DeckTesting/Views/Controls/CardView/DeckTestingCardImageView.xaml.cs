using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using MTGApplication.General.Views.Controls;
using System.Numerics;

namespace MTGApplication.Features.DeckTesting.Views.Controls.CardView;

public sealed partial class DeckTestingCardImageView : DeckTestingCardViewBase
{
  public DeckTestingCardImageView() => InitializeComponent();

  protected override void HoverPreviewUpdate(FrameworkElement sender, PointerRoutedEventArgs e)
  {
    // Dont show preview image if the user is dragging a card
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
      CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });
    else
    {
      CardPreview.Change(this, new(XamlRoot)
      {
        Uri = SelectedFaceUri,
        Coordinates = sender.TransformToVisual(null).TransformPoint(new()).ToVector2() + new Vector2((float)CardPreview.ImageX / 4),
        OffsetOverride = Vector2.Zero
      });
    }
  }
}
