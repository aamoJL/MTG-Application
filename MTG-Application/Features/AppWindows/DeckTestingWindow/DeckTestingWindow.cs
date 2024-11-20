using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.Views;
using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.AppWindows.DeckTestingWindow;

public partial class DeckTestingWindow : ThemedWindow
{
  public DeckTestingWindow(DeckTestingDeck deck)
  {
    Title = "MTG Deck Testing";

    Navigate(typeof(DeckTestingPage), deck);
  }
}
