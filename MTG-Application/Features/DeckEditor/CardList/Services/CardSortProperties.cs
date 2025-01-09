using CommunityToolkit.WinUI.Collections;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System;
using System.Collections.Generic;
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

  public IComparer<object> Comparer => new MTGCardPropertyComparer([PrimarySortProperty, SecondarySortProperty], SortDirection);

  public class MTGCardPropertyComparer(IEnumerable<MTGSortProperty> properties, SortDirection direction) : IComparer<object>
  {
    public int Compare(object? x, object? y)
    {
      var result = 1;

      foreach (var property in properties)
      {
        var cx = GetComparable(x as DeckEditorMTGCard, property);
        var cy = GetComparable(y as DeckEditorMTGCard, property);

        result = cx == cy ? 0 : cx == null ? -1 : cy == null ? +1 : cx.CompareTo(cy);

        if (direction == SortDirection.Descending)
          result *= -1;

        if (result != 0)
          break;
      }

      return result;
    }

    private static IComparable? GetComparable(DeckEditorMTGCard? card, MTGSortProperty sortProperty)
    {
      return sortProperty switch
      {
        MTGSortProperty.CMC => card?.Info.CMC,
        MTGSortProperty.Name => card?.Info.Name,
        MTGSortProperty.Rarity => card?.Info.RarityType,
        MTGSortProperty.Color => card == null ? null : (card.Info.Colors.Length > 1 ? ColorTypes.M : card.Info.Colors[0]),
        MTGSortProperty.Set => card?.Info.SetCode,
        MTGSortProperty.Count => card?.Count,
        MTGSortProperty.Price => card?.Info.Price,
        MTGSortProperty.SpellType => card?.Info.SpellTypes[0],
        _ => null
      };
    }
  }
}
