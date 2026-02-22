using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models;
using System;

namespace MTGApplication.Features.DeckEditor.Models;

/// <summary>
/// MTG card model
/// </summary>
public partial class DeckEditorMTGCard(MTGCardInfo info) : MTGCard(info)
{
  /// <summary>
  /// Card count. Minimum is 1
  /// </summary>
  public int Count
  {
    get;
    set
    {
      value = Math.Max(1, value);
      if (field != value)
      {
        field = value;
        OnPropertyChanged(nameof(Count));
      }
    }
  } = 1;
  [ObservableProperty] public partial string Group { get; set; } = string.Empty;
  [ObservableProperty] public partial CardTag? CardTag { get; set; } = null;

  public DeckEditorMTGCard Copy() => (DeckEditorMTGCard)MemberwiseClone();
}