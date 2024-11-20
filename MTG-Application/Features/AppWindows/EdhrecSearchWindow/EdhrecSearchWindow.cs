using MTGApplication.Features.EdhrecSearch.Views;
using MTGApplication.General.Views.AppWindows;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;

namespace MTGApplication.Features.AppWindows.EdhrecSearchWindow;
public partial class EdhrecSearchWindow : ThemedWindow
{
  public EdhrecSearchWindow(CommanderTheme[] themes)
  {
    Title = "EDHREC Search";
    Width = 550;

    Navigate(typeof(EdhrecSearchPage), parameters: themes);
  }
}
