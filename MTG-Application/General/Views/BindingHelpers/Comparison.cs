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
}
