using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text.Json.Serialization;

namespace MTGApplication.General.Models.Card;

/// <summary>
/// MTG card model
/// </summary>
public partial class DeckEditorMTGCard : ObservableObject
{
  protected int count = 1;

  [JsonConstructor]
  public DeckEditorMTGCard(MTGCardInfo info, int count = 1)
  {
    Info = info;
    Count = count;
  }

  [ObservableProperty] private MTGCardInfo info;

  /// <summary>
  /// Name of the API, that was used to fetch this card
  /// </summary>
  public string APIName => Info.APIName;

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