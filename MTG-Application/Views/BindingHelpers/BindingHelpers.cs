using System.Globalization;

namespace MTGApplication.Views.BindingHelpers
{
  public static class Comparison
  {
    public static bool MoreThan(int more, int than) => more > than;
    public static bool NotNull(object target) => target != null;
    public static bool NotNullOrEmpty(string text) => !string.IsNullOrEmpty(text);
  }

  public static class Format
  {
    public static string EuroToString(float value) => value.ToString("c2", new CultureInfo("fi-FI"));
    public static string ToUpper(string text) => text.ToUpper();
  }

  public static class MTGCardViewModel
  {
    public static double OwnedToOpacity(bool owned) { return owned ? 1 : .5f;}
  }
}
