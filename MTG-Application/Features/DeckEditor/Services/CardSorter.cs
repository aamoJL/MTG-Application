using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Collections;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Views.Controls;
using System;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.Services;

public partial class CardSorter : ObservableObject, IValueSorter<DeckCardViewModel>
{
  public CardSortProperties SortProperties
  {
    get;
    private set
    {
      field = value;
      OnPropertyChanged(nameof(Comparer));
    }
  } = new();

  public IComparer<DeckCardViewModel> Comparer => SortProperties.Comparer;

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