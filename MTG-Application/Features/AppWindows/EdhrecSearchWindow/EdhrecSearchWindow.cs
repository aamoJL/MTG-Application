using MTGApplication.Features.EdhrecSearch.Views;
using MTGApplication.General.Views.AppWindows;

namespace MTGApplication.Features.AppWindows.EdhrecSearchWindow;
public class EdhrecSearchWindow : ThemedWindow
{
  public EdhrecSearchWindow()
  {
    Title = "EDHREC Search";
    Width = 550;

    Navigate(typeof(EdhrecSearchPage));
  }
}
