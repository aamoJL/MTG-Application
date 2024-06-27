namespace MTGApplication.General.Views.BindingHelpers;

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
