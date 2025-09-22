using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.General.ViewModels;
using System;

namespace MTGApplication.Features.DeckTesting.UseCases;

public class ShuffleDeck(DeckTestingPageViewModel viewmodel) : SyncCommand
{
  public DeckTestingPageViewModel Viewmodel { get; } = viewmodel;

  protected override void Execute()
  {
    var rng = new Random();
    var list = Viewmodel.Library;
    var n = list.Count;
    while (n > 1)
    {
      n--;
      var k = rng.Next(n + 1);
      (list[n], list[k]) = (list[k], list[n]);
    }
  }
}
