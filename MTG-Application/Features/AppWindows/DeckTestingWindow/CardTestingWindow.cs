using MTGApplication.Features.DeckTesting.Views;
using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.AppWindows.DeckTestingWindow;

public class CardTestingWindow : ThemedWindow
{
  public CardTestingWindow()
  {
    Title = "MTG Deck Testing";

    Navigate(typeof(DeckTestingPage));
  }
}
