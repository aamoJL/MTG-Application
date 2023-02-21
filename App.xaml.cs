using Microsoft.UI.Xaml;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Pages;
using MTGApplication.Services;
using MTGApplication.Database;
using Microsoft.EntityFrameworkCore;

namespace MTGApplication
{
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  public partial class App : Application
  {
    public static Window MainWindow { get; private set; }
    // Used for Dialogs
    public static FrameworkElement MainRoot { get; private set; }
    // Used for AppData subfolder
    public static string CompanyName { get; } = "aamo"; // TODO: Move to somewhere

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
      using (var db = new CardDbContextFactory().CreateDbContext())
      {
        db.Database.Migrate();
      }

      MainWindow = new MainWindow
      {
        Title = "MTG Application",
      };

      Frame rootFrame = new();
      MainWindow.Content = rootFrame;
      rootFrame.Navigate(typeof(MainPage), args.Arguments);

      MainWindow.Activate();

      MainRoot = MainWindow.Content as FrameworkElement;

      IO.InitDirectories();
      LiveCharts.Configure(config =>
        config.AddSkiaSharp().AddDefaultMappers().AddLightTheme()
      );
    }
  }
}
