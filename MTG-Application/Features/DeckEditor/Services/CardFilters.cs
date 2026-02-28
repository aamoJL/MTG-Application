using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Views.Controls;
using System;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplication.Features.DeckEditor.Services;

/// <summary>
/// Class that has properties to filter MTG card lists
/// </summary>
public partial class CardFilters : ObservableObject, IValueFilter<DeckCardViewModel>
{
  public enum ColorGroups { All, Mono, Multi }

  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial string NameText { get; set; } = string.Empty;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial string TypeText { get; set; } = string.Empty;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial string OracleText { get; set; } = string.Empty;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial bool White { get; set; } = true;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial bool Blue { get; set; } = true;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial bool Black { get; set; } = true;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial bool Red { get; set; } = true;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial bool Green { get; set; } = true;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial bool Colorless { get; set; } = true;
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial ColorGroups ColorGroup { get; set; } = ColorGroups.All; // All, Mono, Multi
  [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied), nameof(ValidationPredicate))]
  public partial double Cmc { get; set; } = double.NaN;

  public Predicate<DeckCardViewModel> ValidationPredicate => CardValidation;

  /// <summary>
  /// Returns <see langword="true"/> if any of the filter properties has been changed from the default value
  /// </summary>
  public bool FiltersApplied => !string.IsNullOrEmpty(NameText) || !string.IsNullOrEmpty(TypeText) || !string.IsNullOrEmpty(OracleText)
    || !White || !Blue || !Black || !Red || !Green || !Colorless || ColorGroup != ColorGroups.All || !double.IsNaN(Cmc);

  /// <summary>
  /// returns <see langword="true"/> if the given <paramref name="card"/> is valid with the selected filters
  /// </summary>
  public bool CardValidation(DeckCardViewModel? card)
  {
    if (card == null)
      return false;

    if (!FiltersApplied)
      return true;

    if (card.Info.Name.Contains(NameText, StringComparison.OrdinalIgnoreCase)
      && card.Info.TypeLine.Contains(TypeText, StringComparison.OrdinalIgnoreCase)
      && (card.Info.FrontFace.OracleText.Contains(OracleText, StringComparison.OrdinalIgnoreCase)
      || (card.Info.BackFace != null && card.Info.BackFace.OracleText.Contains(OracleText, StringComparison.OrdinalIgnoreCase)))
      && (White || !card.Info.Colors.Contains(ColorTypes.W))
      && (Blue || !card.Info.Colors.Contains(ColorTypes.U))
      && (Black || !card.Info.Colors.Contains(ColorTypes.B))
      && (Red || !card.Info.Colors.Contains(ColorTypes.R))
      && (Green || !card.Info.Colors.Contains(ColorTypes.G))
      && (Colorless || !card.Info.Colors.Contains(ColorTypes.C))
      && (ColorGroup == ColorGroups.All
        || ColorGroup == ColorGroups.Mono && card.Info.Colors.Count == 1
        || (ColorGroup == ColorGroups.Multi && card.Info.Colors.Count > 1))
      && (double.IsNaN(Cmc) || card.Info.CMC == Cmc))
    {
      return true;
    }
    else return false;
  }

  /// <summary>
  /// Reset filter properties to the default values
  /// </summary>
  [RelayCommand(CanExecute = nameof(CanReset))]
  private void Reset()
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
  private void ChangeColorGroup(string group)
  {
    if (Enum.TryParse(group, out ColorGroups colorGroup))
      ColorGroup = colorGroup;
  }

  private bool CanReset() => FiltersApplied;
}