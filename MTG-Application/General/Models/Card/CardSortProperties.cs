using CommunityToolkit.WinUI.UI;
using System;
using System.Collections;
using static MTGApplication.General.Models.Card.CardSortProperties;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.General.Models.Card;

/// <summary>
/// Record that has properties to sort MTG card lists
/// </summary>
public record CardSortProperties(
  MTGSortProperty PrimarySortProperty = MTGSortProperty.CMC,
  MTGSortProperty SecondarySortProperty = MTGSortProperty.Name,
  SortDirection SortDirection = SortDirection.Ascending)
{
  public enum MTGSortProperty { CMC, Name, Rarity, Color, Set, Count, Price, SpellType }

  public class MTGCardComparer : IComparer
  {
    public MTGCardComparer(MTGSortProperty sortProperty) => SortProperty = sortProperty;

    public MTGSortProperty SortProperty { get; set; }

    // TODO: unit test
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
        MTGSortProperty.Color => card.Info.Colors.Length > 1 ? ColorTypes.M : card.Info.Colors[0],
        MTGSortProperty.Set => card.Info.SetCode,
        MTGSortProperty.Count => card.Count,
        MTGSortProperty.Price => card.Info.Price,
        MTGSortProperty.SpellType => card.Info.SpellTypes[0],
        _ => null
      };
    }
  }
}
