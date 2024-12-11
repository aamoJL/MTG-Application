using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Collections;
using System;

namespace MTGApplication.Features.DeckEditor.CardList.Services;

public partial class CardSorter : ObservableObject
{
  [ObservableProperty] public partial CardSortProperties SortProperties { get; set; } = new();

  [RelayCommand]
  private void ChangeSortDirection(string direction)
  {
    if (Enum.TryParse(direction, true, out SortDirection sortDirection))
      SortProperties = SortProperties with { SortDirection = sortDirection };
  }

  [RelayCommand]
  private void ChangePrimarySortProperty(string property)
  {
    if (Enum.TryParse(property, true, out CardSortProperties.MTGSortProperty primaryProperty))
      SortProperties = SortProperties with { PrimarySortProperty = primaryProperty };
  }

  [RelayCommand]
  private void ChangeSecondarySortProperty(string property)
  {
    if (Enum.TryParse(property, true, out CardSortProperties.MTGSortProperty secondaryProperty))
      SortProperties = SortProperties with { SecondarySortProperty = secondaryProperty };
  }
}