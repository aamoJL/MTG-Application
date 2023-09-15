using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MTGApplication.Enums;
using static MTGApplication.Models.MTGCard;

namespace MTGApplication.Services;

/// <summary>
/// Service that has methods to handle MTG Card operations
/// </summary>
public static partial class MTGService
{
  /// <summary>
  /// Returns exportable string of the cards using the <paramref name="exportProperty"/>
  /// </summary>
  public static string GetExportString(MTGCard[] cards, string exportProperty = "Name")
  {
    StringBuilder stringBuilder = new();
    foreach (var item in cards)
    {
      if (exportProperty == "Name")
      {
        stringBuilder.AppendLine($"{item.Count} {item.Info.Name}");
      }
      else if (exportProperty == "Id")
      {
        stringBuilder.AppendLine($"{item.Count} {item.Info.ScryfallId}");
      }
    }

    return stringBuilder.ToString();
  }

  /// <summary>
  /// Separates the card types from the <paramref name="typeLine"/> to a <see cref="SpellType"/> array.
  /// </summary>
  public static SpellType[] GetSpellTypes(string typeLine)
  {
    List<SpellType> types = new();
    var typeStrings = typeLine.Split('\u0020'); // 'Space'

    foreach (var typeString in typeStrings)
    {
      if (Enum.TryParse(typeString, true, out SpellType spellType))
      {
        types.Add(spellType);
      }
    }

    if (types.Count == 0) { types.Add(SpellType.Other); }
    return types.OrderBy(x => x).ToArray();
  }

  /// <summary>
  /// Returns all the <see cref="ColorTypes"/> that the given faces have
  /// </summary>
  public static ColorTypes[] GetColors(CardFace frontFace, CardFace? backFace)
  {
    var colors = new List<ColorTypes>();

    foreach (var color in frontFace.Colors)
    {
      if (!colors.Contains(color)) { colors.Add(color); }
    }

    if (backFace != null)
    {
      foreach (var color in backFace?.Colors)
      {
        if (!colors.Contains(color)) { colors.Add(color); }
      }
    }

    // Card is colorless if it has no other colors
    if (colors.Count == 0) { colors.Add(ColorTypes.C); }
    return colors.ToArray();
  }
}

// MTG Service enums
public static partial class MTGService
{
  public enum RarityTypes
  {
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Mythic = 3,
    Special = 4,
    Bonus = 5,
  }

  public enum ColorTypes
  {
    W, U, B, R, G, M, C
  }

  public enum SpellType
  {
    Land,
    Creature,
    Artifact,
    Enchantment,
    Planeswalker,
    Instant,
    Sorcery,
    Other
  }

  public enum CardSide
  {
    Front, Back
  }

  /// <summary>
  /// Returns given <paramref name="color"/> character's full name
  /// </summary>
  public static string GetFullName(this ColorTypes color)
  {
    return color switch
    {
      ColorTypes.W => "White",
      ColorTypes.U => "Blue",
      ColorTypes.B => "Black",
      ColorTypes.R => "Red",
      ColorTypes.G => "Green",
      ColorTypes.M => "Multicolor",
      _ => "Colorless",
    };
  }
}

// MTG Service classes
public static partial class MTGService
{
  /// <summary>
  /// Class that has properties to filter MTG card lists
  /// </summary>
  public partial class MTGCardFilters : ObservableObject
  {
    public enum ColorGroups { All, Mono, Multi }

    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private string nameText = string.Empty;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private string typeText = string.Empty;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private string oracleText = string.Empty;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private bool white = true;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private bool blue = true;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private bool black = true;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private bool red = true;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private bool green = true;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private bool colorless = true;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private ColorGroups colorGroup = ColorGroups.All; // All, Mono, Multi
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
    private double cmc = double.NaN;

    /// <summary>
    /// Returns <see langword="true"/> if any of the filter properties has been changed from the default value
    /// </summary>
    public bool FiltersApplied => !string.IsNullOrEmpty(NameText) || !string.IsNullOrEmpty(TypeText) || !string.IsNullOrEmpty(OracleText)
      || !White || !Blue || !Black || !Red || !Green || !Colorless || ColorGroup != ColorGroups.All || !double.IsNaN(Cmc);

    /// <summary>
    /// returns <see langword="true"/> if the given <paramref name="card"/> is valid with the selected filters
    /// </summary>
    public bool CardValidation(MTGCardViewModel cardViewModel)
    {
      if (cardViewModel.Name.Contains(NameText, StringComparison.OrdinalIgnoreCase)
        && cardViewModel.TypeLine.Contains(TypeText, StringComparison.OrdinalIgnoreCase)
        && (cardViewModel.Model.Info.FrontFace.OracleText.Contains(OracleText, StringComparison.OrdinalIgnoreCase)
        || (cardViewModel.Model.Info.BackFace != null && cardViewModel.Model.Info.BackFace.Value.OracleText.Contains(OracleText, StringComparison.OrdinalIgnoreCase)))
        && (White || !cardViewModel.Colors.Contains(ColorTypes.W))
        && (Blue || !cardViewModel.Colors.Contains(ColorTypes.U))
        && (Black || !cardViewModel.Colors.Contains(ColorTypes.B))
        && (Red || !cardViewModel.Colors.Contains(ColorTypes.R))
        && (Green || !cardViewModel.Colors.Contains(ColorTypes.G))
        && (Colorless || !cardViewModel.Colors.Contains(ColorTypes.C))
        && (ColorGroup == ColorGroups.All
          || ColorGroup == ColorGroups.Mono && cardViewModel.Colors.Length == 1
          || (ColorGroup == ColorGroups.Multi && cardViewModel.Colors.Length > 1))
        && (double.IsNaN(Cmc) || cardViewModel.CMC == Cmc))
      {
        return true;
      }
      else { return false; };
    }

    /// <summary>
    /// Reset filter properties to the default values
    /// </summary>
    [RelayCommand]
    public void Reset()
    {
      NameText = string.Empty;
      TypeText = string.Empty;
      OracleText = string.Empty;
      White = true;
      Blue = true;
      Black = true;
      Red = true;
      Green = true;
      Colorless = true;
      ColorGroup = ColorGroups.All;
      Cmc = double.NaN;
    }

    /// <summary>
    /// Changes <see cref="ColorGroup"/> to the given <paramref name="group"/>
    /// </summary>
    [RelayCommand]
    public void ChangeColorGroup(string group)
    {
      if (Enum.TryParse(group, out ColorGroups colorGroup))
      {
        ColorGroup = colorGroup;
      }
    }
  }

  /// <summary>
  /// Class that has properties to sort MTG card lists
  /// </summary>
  public partial class MTGCardSortProperties : ObservableObject
  {
    [ObservableProperty] public MTGSortProperty primarySortProperty;
    [ObservableProperty] public MTGSortProperty secondarySortProperty;
    [ObservableProperty] public SortDirection sortDirection;
  }
}
