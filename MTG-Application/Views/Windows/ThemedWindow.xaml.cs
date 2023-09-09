using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MTGApplication.Interfaces;

namespace MTGApplication.Views.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ThemedWindow : Window
{
  public ThemedWindow(string iconUri = "Assets/Icon.ico") : base()
  {
    InitializeComponent();
    AppWindow.SetIcon(iconUri);

    Closed += ThemedWindow_Closed;
  }

  /// <summary>
  /// <inheritdoc cref="Window.Content"/>
  /// </summary>
  public new UIElement Content
  {
    get => base.Content;
    set
    {
      base.Content = value;
      // Set theme
      if (value != null)
      {
        (value as FrameworkElement).RequestedTheme = AppConfig.LocalSettings.AppTheme;
        AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged;
      }
    }
  }

  /// <summary>
  /// Window's width
  /// </summary>
  public int Width
  {
    get => AppWindow.Size.Width;
    set => AppWindow.Resize(new(value, Height));
  }

  /// <summary>
  /// Window's height
  /// </summary>
  public int Height
  {
    get => AppWindow.Size.Height;
    set => AppWindow.Resize(new(Width, value));
  }

  private bool close = false; // Temp variable for window closing

  private async void ThemedWindow_Closed(object sender, WindowEventArgs args)
  {
    // The Window will close if the close variable has been set to true.
    // Otherwise the user will be asked to save unsaved changes.
    // If the user does not cancel the closing event, this method will be called again with the close variable set to true.
    if (close) { return; }

    var canceled = false;

    if (Content is ISavable savableContent)
    {
      args.Handled = savableContent.HasUnsavedChanges;
      canceled = !await savableContent.SaveUnsavedChanges();
    }

    if (!canceled)
    {
      close = true;
      args.Handled = false;
      Content = null;
      AppConfig.LocalSettings.PropertyChanged -= LocalSettings_PropertyChanged;
      Close();
    }
  }

  private void LocalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
    {
      (Content as FrameworkElement).RequestedTheme = AppConfig.LocalSettings.AppTheme;
    }
  }
}
