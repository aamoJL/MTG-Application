using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.CardSearch;
public sealed partial class CardSearchPage : Page
{
  public CardSearchPage() => InitializeComponent();

  public CardSearchViewModel ViewModel { get; } = new(App.MTGCardAPI);

  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    // TODO: drag staring
  }

  private void PreviewableCard_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    // TODO: pointer event
  }

  private void PreviewableCard_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    // TODO: pointer event
  }

  private void PreviewableCard_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    // TODO: pointer event
  }
}
