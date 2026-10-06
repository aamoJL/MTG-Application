using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using MTGApplication.Features.AppWindows.DeckBuilderWindow.Views;
using MTGApplication.Features.DeckEditor.Views.Charts.Models;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Cache;
using MTGApplication.General.Services.Databases.Context;
using MTGApplication.General.Services.Importers.CardImporter;
using System.Runtime.InteropServices;

namespace MTGApplication;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
  // TODO: remove if unnecessary.
  // Workaround for Win10 WinUI 3 framerate bug.
  // https://github.com/microsoft/microsoft-ui-xaml/issues/12073
  [LibraryImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
  internal static partial uint TimeBeginPeriod(uint uPeriod);

  public static IMTGCardImporter MTGCardImporter { get; } = new ScryfallAPI();
  public static MemoryCache<Caching.CacheKey> Cache { get; } = new MemoryCache<Caching.CacheKey>();

  /// <summary>
  /// Initializes the singleton application object.  This is the first line of authored code
  /// executed, and as such is the logical equivalent of main() or WinMain().
  /// </summary>
  public App()
  {
    InitializeComponent();

    _ = TimeBeginPeriod(1); // Workaround for Win10 WinUI 3 framerate bug.
  }

  /// <summary>
  /// Invoked when the application is launched normally by the end user.  Other entry points
  /// will be used such as when the application is launched to open a specific file.
  /// </summary>
  /// <param name="args">Details about the launch request and process.</param>
  protected override void OnLaunched(LaunchActivatedEventArgs args)
  {
    DispatcherQueue.GetForCurrentThread().ShutdownStarting += App_ShutdownStarting;

    AppConfig.Initialize();
    Cache.LoadFromFile();

    using (var db = new CardDbContextFactory().CreateDbContext())
    {
      db.Database.Migrate();
    }

    LiveCharts.Configure(config => config
      .AddSkiaSharp()
      .AddDefaultMappers()
      .AddDefaultTheme(theme => theme
        .HasRuleForAxes(axis =>
        {
          axis.LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor);
          (axis as IPolarAxis)?.LabelsBackground = LvcColor.Empty;
        })
        .HasRuleForPieSeries(pie => pie.DataLabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor)))
      );

    new DeckBuilderWindow().Activate();
  }

  private void App_ShutdownStarting(DispatcherQueue sender, DispatcherQueueShutdownStartingEventArgs args)
  {
    var def = args.GetDeferral();
    Cache.SaveToFile();
    def.Complete();
  }
}