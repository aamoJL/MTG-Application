using CommunityToolkit.WinUI.UI;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System;
using System.Collections;
using static MTGApplication.Features.DeckEditor.CardList.Services.CardSortProperties;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplication.Features.DeckEditor.CardList.Services;

/// <summary>
/// Class that has properties to sort MTG card lists
/// </summary>
public record CardSortProperties(
  MTGSortProperty PrimarySortProperty = MTGSortProperty.CMC,
  MTGSortProperty SecondarySortProperty = MTGSortProperty.Name,
  SortDirection SortDirection = SortDirection.Ascending)
{
  public enum MTGSortProperty { CMC, Name, Rarity, Color, Set, Count, Price, SpellType }

  public class MTGCardPropertyComparer(MTGSortProperty sortProperty) : IComparer
  {
    public int Compare(object x, object y)
    {
      var cx = GetComparable(x as DeckEditorMTGCard, sortProperty);
      var cy = GetComparable(y as DeckEditorMTGCard, sortProperty);

      return cx == cy ? 0 : cx == null ? -1 : cy == null ? +1 : cx.CompareTo(cy);
    }

    private static IComparable GetComparable(DeckEditorMTGCard card, MTGSortProperty sortProperty)
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
