using CommunityToolkit.WinUI.Collections;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using System;
using System.Collections.Generic;
using static MTGApplication.Features.DeckEditor.Services.CardSortProperties;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplication.Features.DeckEditor.Services;

/// <summary>
/// Class that has properties to sort MTG card lists
/// </summary>
public record CardSortProperties(
  MTGSortProperty PrimarySortProperty = MTGSortProperty.CMC,
  MTGSortProperty SecondarySortProperty = MTGSortProperty.Name,
  SortDirection SortDirection = SortDirection.Ascending)
{
  public enum MTGSortProperty { CMC, Name, Rarity, Color, Set, Count, Price, SpellType }

  public IComparer<DeckCardViewModel> Comparer => new MTGCardPropertyComparer([PrimarySortProperty, SecondarySortProperty], SortDirection);

  public class MTGCardPropertyComparer(IEnumerable<MTGSortProperty> properties, SortDirection direction) : IComparer<object>
  {
    public int Compare(object? x, object? y)
    {
      if (x is not DeckCardViewModel vmX || y is not DeckCardViewModel vmY)
        throw new InvalidOperationException($"Compared types are not {typeof(DeckCardViewModel)}");

      var result = 1;

      foreach (var property in properties)
      {
        var cx = GetComparable(vmX, property);
        var cy = GetComparable(vmY, property);

        result = cx == cy ? 0 : cx == null ? -1 : cy == null ? +1 : cx.CompareTo(cy);

        if (direction == SortDirection.Descending)
          result *= -1;

        if (result != 0)
          break;
      }

      return result;
    }

    private static IComparable? GetComparable(DeckCardViewModel card, MTGSortProperty sortProperty)
    {
      return sortProperty switch
      {
        MTGSortProperty.CMC => card.Info.CMC,
        MTGSortProperty.Name => card.Info.Name,
        MTGSortProperty.Rarity => card.Info.RarityType,
        MTGSortProperty.Color => card == null ? null : (card.Info.Colors.Count > 1 ? ColorTypes.M : card.Info.Colors[0]),
        MTGSortProperty.Set => card.Info.SetCode,
        MTGSortProperty.Count => card.Count,
        MTGSortProperty.Price => card.Info.Price,
        MTGSortProperty.SpellType => card.Info.SpellTypes[0],
        _ => null
      };
    }
  }
}
