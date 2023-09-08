using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.API;
using MTGApplication.Services;
using MTGApplication.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.Views.Controls.MTGCardPreviewControl;

namespace MTGApplication.Views.Controls;

[ObservableObject]
public sealed partial class CardAPISearchControl : UserControl
{
  public CardAPISearchControl() => InitializeComponent();

  public DeckBuilderAPISearchViewModel SearchViewModel { get; set; }
  public CardPreviewProperties CardPreviewProperties
  {
    get => (CardPreviewProperties)GetValue(CardPreviewPropertiesProperty);
    set => SetValue(CardPreviewPropertiesProperty, value);
  }
  public DialogService DialogService
  {
    get => (DialogService)GetValue(DialogServiceProperty);
    set
    {
      SetValue(DialogServiceProperty, value);
      SearchViewModel = new(new ScryfallAPI(), DialogService);
    }
  }

  public static readonly DependencyProperty CardPreviewPropertiesProperty =
      DependencyProperty.Register("CardPreviewProperties", typeof(CardPreviewProperties), typeof(CardAPISearchControl), new PropertyMetadata(0));

  public static readonly DependencyProperty DialogServiceProperty =
      DependencyProperty.Register("DialogService", typeof(DialogService), typeof(CardAPISearchControl), new PropertyMetadata(0));

  [ObservableProperty] private double searchDesiredItemWidth = 250;

  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCardViewModel vm)
    {
      e.Data.SetText(vm.Model.ToJSON());
      e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
    }
  }

  // Change card preview image to hovered item
  private void PreviewableCard_PointerEntered(object sender, PointerRoutedEventArgs e)
    => CardPreviewProperties.CardViewModel = (sender as FrameworkElement)?.DataContext as MTGCardViewModel;

  private void PreviewableCard_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    // Move card preview image to mouse position when hovering over on list view item.
    // The position is clamped to element size
    var point = e.GetCurrentPoint(null).Position;
    CardPreviewProperties.Coordinates = new((float)point.X, (float)point.Y);
  }

  private void PreviewableCard_PointerExited(object sender, PointerRoutedEventArgs e)
    => CardPreviewProperties.CardViewModel = null;
}
