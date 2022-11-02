using System.Globalization;

namespace MTGApplication.BindingHelpers
{
  public static class Comparison
  {
    public static bool MoreThan(int more, int than) => more > than;
    public static bool NotNull(object target) => target != null;
    public static bool NotNullOrEmpty(string text) => !string.IsNullOrEmpty(text);
    public static string EuroToString(float value) => value.ToString("c2", new CultureInfo("fi-FI"));
  }
}
