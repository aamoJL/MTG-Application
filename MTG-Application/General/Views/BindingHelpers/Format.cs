using System;
using System.Globalization;

namespace MTGApplication.General.Views.BindingHelpers;

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

  /// <summary>
  /// Retruns given default text, if the value is empty or null
  /// </summary>
  public static string ValueOrDefault(string value, string defaultText)
    => string.IsNullOrEmpty(value) ? defaultText : value;
}
