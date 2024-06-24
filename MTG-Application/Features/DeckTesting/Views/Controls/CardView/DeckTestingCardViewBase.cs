using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.Services;
using MTGApplication.General.Views.Controls;
using MTGApplication.General.Views.Extensions;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckTesting.Views.Controls.CardView;

public partial class DeckTestingCardViewBase : BasicCardView<DeckTestingMTGCard>
{
  public DeckTestingCardViewBase() => PointerPressed += DeckTestingCardViewBase_PointerPressed;

  private void DeckTestingCardViewBase_PointerPressed(object sender, PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    {
      var pointerPosition = e.GetCurrentPoint(null).Position;

      CardDragArgs.Completed += OnDragCompleted;
      CardDragArgs.Ended += OnDragEnded;

      DragCardPreview.Change(this, new(XamlRoot)
      {
        Uri = SelectedFaceUri,
      });

      CardDragArgs.Start(Model);
    }
  }

  protected override void HoverPreviewUpdate(FrameworkElement sender, PointerRoutedEventArgs e)
  {
    if (CardDragArgs.IsDragging)
    {
      CardPreview.Change(this, new(XamlRoot) { Uri = null });
      return; // Disable card preview if card is being dragged
    }

    base.HoverPreviewUpdate(sender, e);
  }

  private void OnDragCompleted(DeckTestingMTGCard item)
    => (this.FindParentByType<ListViewBase>()?.ItemsSource as ObservableCollection<DeckTestingMTGCard>).Remove(item);

  private void OnDragEnded()
  {
    CardDragArgs.Completed -= OnDragCompleted;
    CardDragArgs.Ended -= OnDragEnded;
  }
}
