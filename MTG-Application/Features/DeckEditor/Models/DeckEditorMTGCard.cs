using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MTGApplication.General.Models.Card;

/// <summary>
/// MTG card model
/// </summary>
public partial class DeckEditorMTGCard(MTGCardInfo info, int count = 1) : ObservableObject
{
  protected int count = Math.Max(1, count);
  [ObservableProperty] private MTGCardInfo info = info;

  /// <summary>
  /// Name of the API, that was used to fetch this card
  /// </summary>
  public string ImporterName => Info.ImporterName;

  /// <summary>
  /// Card count. Minimum is 1
  /// </summary>
  public int Count
  {
    get => count;
    set
    {
      value = Math.Max(1, value);
      if (count != value)
      {
        count = value;
        OnPropertyChanged(nameof(Count));
      }
    }
  }
}