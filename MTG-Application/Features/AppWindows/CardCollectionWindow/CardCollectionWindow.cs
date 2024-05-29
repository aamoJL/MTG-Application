using MTGApplication.Features.CardCollection;
using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.AppWindows.CardCollectionWindow;

public class CardCollectionWindow : ThemedWindow
{
  public CardCollectionWindow()
  {
    Title = "MTG Card Collections";

    Navigate(typeof(CardCollectionPage));
  }
}
