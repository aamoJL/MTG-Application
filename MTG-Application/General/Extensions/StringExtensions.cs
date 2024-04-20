using System.Text.RegularExpressions;

namespace MTGApplication.General.Extensions;

public static class StringExtensions
{
  /// <summary>
  /// Returns a copy of this string converted to kebab case.
  /// </summary>
  /// <returns>Example: this-is-kebab-case-text</returns>
  public static string ToKebabCase(this string value)
  {
    value = Regex.Replace(value, @"\s", "-"); // change spaces to dashes
    value = Regex.Replace(value, @"[^0-9a-zA-Z-]", string.Empty); // remove all non-alphanumeric, non-dash characters
    value = Regex.Replace(value, @"[-]{2,}", "-"); // changes subsequent dashes to single dash

    return value;
  }
}
