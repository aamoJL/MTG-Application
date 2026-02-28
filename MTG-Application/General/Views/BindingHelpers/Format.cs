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

  /// <inheritdoc cref="EuroToString(float)"/>
  public static string EuroToString(double value, int digits) => Math.Round(value, digits).ToString("c2", new CultureInfo("fi-FI"));
}
