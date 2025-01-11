using System.Text.RegularExpressions;

namespace MTGApplication.General.Extensions;

public static partial class StringExtensions
{
  [GeneratedRegex(@"\s")]
  private static partial Regex GetWhiteSpaces();
  [GeneratedRegex(@"[^0-9a-zA-Z-]")]
  private static partial Regex GetNonAlphanumericAndNonDashCharacters();
  [GeneratedRegex(@"[-]{2,}")]
  private static partial Regex GetSubsequentDashes();

  /// <summary>
  /// Returns a copy of this string converted to kebab case.
  /// </summary>
  /// <returns>Example: this-is-kebab-case-text</returns>
  public static string ToKebabCase(this string value)
  {
    value = GetWhiteSpaces().Replace(value, "-");
    value = GetNonAlphanumericAndNonDashCharacters().Replace(value, string.Empty);
    value = GetSubsequentDashes().Replace(value, "-");

    return value;
  }
}