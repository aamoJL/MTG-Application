using MTGApplication.Features.DeckTesting.Models;
using System;

namespace MTGApplication.Features.DeckTesting.Services;
public static class CardDragArgs
{
  public static float UndroppableOpacity { get; } = .3f;
  public static float DroppableOpacity { get; } = .8f;

  public static DeckTestingMTGCard Item { get; private set; }
  public static bool IsDragging { get; private set; } = false;

  public static event Action Started;
  public static event Action<DeckTestingMTGCard> Completed;
  public static event Action Canceled;
  public static event Action Ended;

  public static void Start(DeckTestingMTGCard item)
  {
    Item = item;
    IsDragging = true;

    Started?.Invoke();
  }

  public static void Complete()
  {
    Completed?.Invoke(Item);

    End();
  }

  public static void Cancel()
  {
    Canceled?.Invoke();

    End();
  }

  private static void End()
  {
    Item = null;
    IsDragging = false;

    Ended?.Invoke();
  }
}
