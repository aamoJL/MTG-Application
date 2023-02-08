using Microsoft.UI.Xaml;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Pages;
using MTGApplication.API;
using Microsoft.EntityFrameworkCore;
using MTGApplication.Database;

namespace MTGApplication
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
  {
    // Used for Dialogs
    public static FrameworkElement MainRoot { get; private set; }
    public static MTGCardAPI CardAPI { get; private set; } = new ScryfallAPI();
    
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
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
      using (var db = new CardDatabaseContext())
      {
        db.Database.Migrate();
      }

      m_window = new MainWindow();
      
      Frame rootFrame = new();
      m_window.Content = rootFrame;
      rootFrame.Navigate(typeof(MainPage), args.Arguments);
      
      m_window.Activate();

      MainRoot = m_window.Content as FrameworkElement;

      IO.InitDirectories();
      LiveCharts.Configure(config =>
        config.AddSkiaSharp().AddDefaultMappers().AddLightTheme()
      );
    }

    private Window m_window;
  }
}
