using MTGApplication.Features.EdhrecSearch.Views;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Views.AppWindows;

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
