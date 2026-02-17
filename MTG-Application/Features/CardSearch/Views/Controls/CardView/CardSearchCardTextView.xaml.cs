using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using MTGApplication.General.Views.Controls;

namespace MTGApplication.Features.CardSearch.Views.Controls.CardView;

public sealed partial class CardSearchCardTextView : CardSearchCardViewBase
{
  public CardSearchCardTextView() : base() => InitializeComponent();

  protected override void OnPointerMoved(PointerRoutedEventArgs e)
  {
    base.OnPointerMoved(e);

    HoverPreviewUpdate(this, e);
  }

  protected override void OnPointerExited(PointerRoutedEventArgs e)
  {
    base.OnPointerExited(e);

    CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });
  }

  private void HoverPreviewUpdate(FrameworkElement _, PointerRoutedEventArgs e)
  {
    var point = e.GetCurrentPoint(null).Position;

    CardPreview.Change(this, new(XamlRoot)
    {
      Uri = SelectedFaceUri,
      Coordinates = new((float)point.X, (float)point.Y)
    });
  }
}
