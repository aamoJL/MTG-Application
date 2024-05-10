using System.Collections.Generic;
using static MTGApplication.General.Models.Card.MTGCard;

namespace MTGApplication.General.Models.Card;

public static class ColorTypesExtensions
{
  public static Dictionary<ColorTypes, string> ColorNames { get; } = new()
  {
    {ColorTypes.W, "White" },
    {ColorTypes.U, "Blue"},
    {ColorTypes.B, "Black"},
    {ColorTypes.R, "Red"},
    {ColorTypes.G, "Green"},
    {ColorTypes.M, "Multicolor"},
    {ColorTypes.C, "Colorless"},
  };

  /// <summary>
  /// Returns given <paramref name="color"/> character's full name
  /// </summary>
  public static string GetFullName(this ColorTypes color)
  {
    ColorNames.TryGetValue(color, out var name);
    return name ?? string.Empty;
  }
}