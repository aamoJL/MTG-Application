using MTGApplication.Features.DeckTesting.Views;
using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.AppWindows.DeckTestingWindow;

public class DeckTestingWindow : ThemedWindow
{
  public DeckTestingWindow()
  {
    Title = "MTG Deck Testing";

    Navigate(typeof(DeckTestingPage));
  }
}
