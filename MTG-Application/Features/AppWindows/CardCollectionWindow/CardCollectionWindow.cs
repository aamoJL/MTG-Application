using MTGApplication.Features.CardCollectionEditor.Editor.Views;
using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.AppWindows.CardCollectionWindow;

public partial class CardCollectionWindow : ThemedWindow
{
  public CardCollectionWindow()
  {
    Title = "MTG Card Collections";

    Navigate(typeof(CardCollectionPage));
  }
}
