using Microsoft.UI.Xaml;

namespace MTGApplication.General.Views.BindingHelpers;

/// <summary>
/// Visibility helper methods
/// </summary>
public static class VisibilityHelpers
{
  /// <summary>
  /// Returns the inverted visibility value of the given visibility
  /// </summary>
  public static Visibility VisibilityInversion(Visibility visibility) => visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

  /// <summary>
  /// Returns <see cref="Visibility.Visible"/>, if the <paramref name="value"/> is true
  /// </summary>
  public static Visibility BooleanToVisibility(bool value) => value ? Visibility.Visible : Visibility.Collapsed;

  /// <summary>
  /// Returns <see cref="Visibility.Visible"/>, if the <paramref name="value"/> equals <paramref name="comparison"/>
  /// </summary>
  public static Visibility IntToVisibility(int value, int comparison) => value == comparison ? Visibility.Visible : Visibility.Collapsed;
}