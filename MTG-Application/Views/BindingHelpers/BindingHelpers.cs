using System.Globalization;

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
  /// Returns <see langword="true"/> if the <paramref name="text"/> is not <see langword="null"/> or <see cref="string.Empty"/>
  /// </summary>
  public static bool NotNullOrEmpty(string text) => !string.IsNullOrEmpty(text);
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
  /// Converts <paramref name="text"/> to uppercase
  /// </summary>
  public static string ToUpper(string text) => text.ToUpper();
}

/// <summary>
/// <see cref="MTGCardViewModel"/> helper methods
/// </summary>
public static class MTGCardViewModel
{
  /// <summary>
  /// Returns 1, if <paramref name="owned"/> is <see langword="true"/>, otherwise returns .5f
  /// </summary>
  public static double OwnedToOpacity(bool owned) => owned ? 1 : .5f;
}
