using Microsoft.UI.Xaml;
using System;
using System.Globalization;
using MTGApplication.ViewModels;

namespace MTGApplication.Views.BindingHelpers;

/// <summary>
/// Value comparison helper methods
/// </summary>
public static class Comparison
{
  /// <summary>
  /// Returns <see langword="true"/> if the <paramref name="more"/> is more than <paramref name="than"/>
  /// </summary>
  public static bool MoreThan(int more, int than) => more > than;

  /// <summary>
  /// Returns <see langword="true"/> if the <paramref name="target"/> is not <see langword="null"/>
  /// </summary>
  public static bool NotNull(object target) => target != null;

  /// <summary>
  /// Returns <see langword="true"/> if the <paramref name="text"/> is not <see langword="null"/> or empty
  /// </summary>
  public static bool NotNullOrEmpty(string text) => !string.IsNullOrEmpty(text);

  /// <summary>
  /// Retruns <see langword="true"/> if the <paramref name="value"/> equals <paramref name="target"/>
  /// </summary>
  public static bool Equals(int value, int target) => value == target;
}

/// <summary>
/// Value formatting helper methods
/// </summary>
public static class Format
{
  /// <summary>
  /// Returns the <paramref name="value"/> as a euro currency formatted <see cref="string"/>.
  /// The string format will be "0,00 €"
  /// </summary>
  public static string EuroToString(float value) => value.ToString("c2", new CultureInfo("fi-FI"));

  /// <summary>
  /// Returns the <paramref name="value"/> as a euro currency formatted <see cref="string"/>.
  /// The string format will be "0,00 €"
  /// </summary>
  public static string EuroToString(float value, int digits) => ((float)Math.Round(value, digits)).ToString("c2", new CultureInfo("fi-FI"));

  /// <summary>
  /// Converts <paramref name="text"/> to uppercase
  /// </summary>
  public static string ToUpper(string text) => text.ToUpper();
}

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

/// <summary>
/// <see cref="MTGCardViewModel"/> helper methods
/// </summary>
public static class MTGCardViewModelHelpers
{
  /// <summary>
  /// Returns 1, if <paramref name="owned"/> is <see langword="true"/>, otherwise returns .5f
  /// </summary>
  public static double OwnedToOpacity(bool owned) => owned ? 1 : .5f;
}
