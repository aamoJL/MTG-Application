using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.Views;

public partial class DeckBuilderWindow : ThemedWindow
{
  public DeckBuilderWindow() : base()
  {
    Title = "Deck Builder";
    Navigate(typeof(DeckBuilderPage));
  }
}
