namespace MTGApplication.Features.CardCollection.Controls;
public sealed partial class CardCollectionCardImageView : CardCollectionCardViewBase
{
  public static double OwnedToOpacity(bool owned) => owned ? 1 : .5f;
  
  public CardCollectionCardImageView() => InitializeComponent();

  private void GridViewItemImage_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
  {

  }

  private void GridViewItemImage_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
  {

  }
}