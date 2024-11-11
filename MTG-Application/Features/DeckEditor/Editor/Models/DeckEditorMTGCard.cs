using MTGApplication.General.Models;
using System;

namespace MTGApplication.Features.DeckEditor.Editor.Models;

/// <summary>
/// MTG card model
/// </summary>
public partial class DeckEditorMTGCard(MTGCardInfo info, int count = 1) : MTGCard(info)
{
  protected int count = LimitCount(count);

  /// <summary>
  /// Card count. Minimum is 1
  /// </summary>
  public int Count
  {
    get => count;
    set
    {
      value = LimitCount(value);
      if (count != value)
      {
        count = value;
        OnPropertyChanged(nameof(Count));
      }
    }
  }

  /// <summary>
  /// Card's group name
  /// </summary>
  public string Group { get; set; } = string.Empty;

  private static int LimitCount(int value) => Math.Max(1, value);
}