using CommunityToolkit.WinUI.UI;
using System;
using System.Collections;
using static MTGApplication.General.Models.Card.MTGCardSortProperties;

namespace MTGApplication.General.Models.Card;

/// <summary>
/// Record that has properties to sort MTG card lists
/// </summary>
public record MTGCardSortProperties(
  MTGSortProperty PrimarySortProperty,
  MTGSortProperty SecondarySortProperty,
  SortDirection SortDirection)
{
  public enum MTGSortProperty { CMC, Name, Rarity, Color, Set, Count, Price, SpellType }

  public class MTGCardComparer : IComparer
  {
    public MTGCardComparer(MTGSortProperty sortProperty) => SortProperty = sortProperty;

    public MTGSortProperty SortProperty { get; }

    public int Compare(object x, object y)
    {
      var cx = GetComparable(x as MTGCard, SortProperty);
      var cy = GetComparable(y as MTGCard, SortProperty);

      return cx == cy ? 0 : cx == null ? -1 : cy == null ? +1 : cx.CompareTo(cy);
    }

    private static IComparable GetComparable(MTGCard card, MTGSortProperty sortProperty)
    {
      return sortProperty switch
      {
        MTGSortProperty.CMC => card.Info.CMC,
        MTGSortProperty.Name => card.Info.Name,
        MTGSortProperty.Rarity => card.Info.RarityType,
        MTGSortProperty.Color => card.ColorType,
        MTGSortProperty.Set => card.Info.SetCode,
        MTGSortProperty.Count => card.Count,
        MTGSortProperty.Price => card.Info.Price,
        MTGSortProperty.SpellType => card.PrimarySpellType,
        _ => card.Info.Name
      };
    }
  }
}
