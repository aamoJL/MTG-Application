using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MTGApplication.Interfaces;

namespace MTGApplication.Views.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ThemedWindow : Window
{
  private static readonly string DEFAULT_ICON_URI = "Assets/Icon.ico";

  private bool _close = false; // Temp variable for window closing

  public ThemedWindow() : base()
  {
    InitializeComponent();

    IconUri = DEFAULT_ICON_URI;

    Closed += ThemedWindow_Closed;
    AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged;
  }

  public string IconUri { set => AppWindow.SetIcon(value); }

  /// <summary>
  /// <inheritdoc cref="Window.Content"/>
  /// </summary>
  public new UIElement Content
  {
    get => base.Content;
    set
    {
      base.Content = value;

      if (value is FrameworkElement element)
        element.RequestedTheme = AppConfig.LocalSettings.AppTheme; // Set theme
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

  private void LocalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (Content is not FrameworkElement element) return;

    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      element.RequestedTheme = AppConfig.LocalSettings.AppTheme;
  }

  private async void ThemedWindow_Closed(object sender, WindowEventArgs args)
  {
    // The Window will close if the close variable has been set to true.
    // Otherwise the user will be asked to save unsaved changes.
    // If the user does not cancel the closing event, this method will be called again with the close variable set to true.
    if (_close) { return; }

    var canceled = false;

    if (Content is ISavable savableContent)
    {
      args.Handled = savableContent.HasUnsavedChanges;
      canceled = !await savableContent.SaveUnsavedChanges();
    }

    if (!canceled)
    {
      _close = true;
      args.Handled = false;
      Content = null;
      AppConfig.LocalSettings.PropertyChanged -= LocalSettings_PropertyChanged;
      Closed -= ThemedWindow_Closed;
      Close();
    }
  }
}
