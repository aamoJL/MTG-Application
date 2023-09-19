using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.API;
using MTGApplication.Interfaces;
using MTGApplication.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.Views.Controls.MTGCardPreviewControl;

namespace MTGApplication.Views.Controls;

[ObservableObject]
public sealed partial class CardAPISearchControl : UserControl
{
  public CardAPISearchControl()
  {
    InitializeComponent();
    SearchViewModel.OnGetDialogWrapper += (s, args) =>
    {
      if (XamlRoot.Content is IDialogPresenter presenter && presenter.DialogWrapper != null)
      {
        args.DialogWrapper = presenter.DialogWrapper;
      }
    };
  }

  #region Properties
  [ObservableProperty] private double searchDesiredItemWidth = 250;

  public DeckBuilderAPISearchViewModel SearchViewModel { get; } = new(new ScryfallAPI());
  public CardPreviewProperties CardPreviewProperties
  {
    get => (CardPreviewProperties)GetValue(CardPreviewPropertiesProperty);
    set => SetValue(CardPreviewPropertiesProperty, value);
  }
  #endregion

  #region Dependency Properties
  public static readonly DependencyProperty CardPreviewPropertiesProperty =
      DependencyProperty.Register("CardPreviewProperties", typeof(CardPreviewProperties), typeof(CardAPISearchControl), new PropertyMetadata(0));
  #endregion

  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCardViewModel vm)
    {
      e.Data.SetText(vm.Model.ToJSON());
      e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
    }
  }

  private void PreviewableCard_PointerEntered(object sender, PointerRoutedEventArgs e)
    // Change card preview image to hovered item
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