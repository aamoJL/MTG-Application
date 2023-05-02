using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using MTGApplication.Database;
using MTGApplication.Interfaces;
using MTGApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
  public class WindowClosingEventArgs : EventArgs
  {
    public List<ISavable> ClosingTasks { get; set; } = new();

    public WindowClosingEventArgs() { }
  }

  public static Window MainWindow { get; private set; }
  public static event EventHandler<WindowClosingEventArgs> Closing;

  private bool close = false; // Variable for window close event so the window will close when all tasks has been completed

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

    MainWindow = new MainWindow
    {
      Title = "MTG Application",
    };

    (MainWindow.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark;

    MainWindow.Activate();
    MainWindow.Closed += MainWindow_Closed;

    DialogService.DialogRoot = MainWindow.Content as FrameworkElement;

    IOService.InitDirectories();

    LiveCharts.Configure(config =>
      config.AddSkiaSharp().AddDefaultMappers().AddLightTheme()
    );
  }

  private async void MainWindow_Closed(object sender, WindowEventArgs args)
  {
    // The Window will close if the close variable has been set to true.
    // Otherwise the user will be asked to save unsaved changes.
    // If the user does not cancel the closing event, this method will be called again with the close variable set to true.
    if (close)
    { return; }

    var closingArgs = new WindowClosingEventArgs();
    Closing?.Invoke(this, closingArgs);

    args.Handled = closingArgs.ClosingTasks.FirstOrDefault(x => x.HasUnsavedChanges is true) != null;
    var canceled = false;

    foreach (var task in closingArgs.ClosingTasks)
    {
      canceled = !await task.SaveUnsavedChanges();
      if (canceled)
      { break; }
    }

    if (!canceled)
    {
      close = true;
      args.Handled = false;
      MainWindow.Close();
    }
  }
}
