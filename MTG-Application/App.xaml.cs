using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using MTGApplication.Features.AppWindows.DeckBuilderWindow;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Context;
using MTGApplication.General.Services.Importers.CardImporter;

namespace MTGApplication;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
  public static MTGCardImporter MTGCardImporter { get; } = new ScryfallAPI();

  /// <summary>
  /// Initializes the singleton application object.  This is the first line of authored code
  /// executed, and as such is the logical equivalent of main() or WinMain().
  /// </summary>
  public App() => InitializeComponent();

  /// <summary>
  /// Invoked when the application is launched normally by the end user.  Other entry points
  /// will be used such as when the application is launched to open a specific file.
  /// </summary>
  /// <param name="args">Details about the launch request and process.</param>
  protected override void OnLaunched(LaunchActivatedEventArgs args)
  {
    AppConfig.Initialize();

    using (var db = new CardDbContextFactory().CreateDbContext())
    {
      db.Database.Migrate();
    }

    new DeckBuilderWindow().Activate();

    LiveCharts.Configure(config => config.AddSkiaSharp().AddDefaultMappers());
  }
}
