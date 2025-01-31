using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models;
using System;

namespace MTGApplication.Features.DeckEditor.Editor.Models;

/// <summary>
/// MTG card model
/// </summary>
public partial class DeckEditorMTGCard(MTGCardInfo info, int count = 1) : MTGCard(info)
{
  /// <summary>
  /// Card count. Minimum is 1
  /// </summary>
  public int Count
  {
    get;
    set
    {
      value = LimitCount(value);
      if (field != value)
      {
        field = value;
        OnPropertyChanged(nameof(Count));
      }
    }
  } = LimitCount(count);

  [ObservableProperty] public partial string Group { get; set; } = string.Empty;

  private static int LimitCount(int value) => Math.Max(1, value);
}