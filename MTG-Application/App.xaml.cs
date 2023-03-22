using Microsoft.UI.Xaml;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Pages;
using MTGApplication.Services;
using MTGApplication.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTGApplication.Interfaces;

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

    // Variable for window close event so the window will close when all tasks has been compleated
    private bool close = false;

    public class WindowClosingEventArgs : EventArgs
    {
      public List<ISavable> ClosingTasks { get; set; } = new();
      //public bool? Canceled => Close.FirstOrDefault(x => x is false);

      public WindowClosingEventArgs() { }
    }

    public static event EventHandler<WindowClosingEventArgs> Closing;

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
      AppConfig.Init();

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
      MainWindow.Closed += MainWindow_Closed;

      MainRoot = MainWindow.Content as FrameworkElement;

      IO.InitDirectories();
      LiveCharts.Configure(config =>
        config.AddSkiaSharp().AddDefaultMappers().AddLightTheme()
      );
    }

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
    {
      if (close) { return; }

      var closingArgs = new WindowClosingEventArgs();
      Closing?.Invoke(this, closingArgs);

      args.Handled = closingArgs.ClosingTasks.FirstOrDefault(x => x.HasUnsavedChanges is true) != null;
      bool canceled = false; 

      foreach (var task in closingArgs.ClosingTasks)
      {
        canceled = !await task.SaveUnsavedChanges();
        if (canceled) { break; }
      }
      
      if(!canceled)
      {
        close = true;
        args.Handled = false;
        MainWindow.Close();
      }
    }
  }
}
