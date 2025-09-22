using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckTesting.UseCases;

public class StartNewTurn(DeckTestingPageViewModel viewmodel) : SyncCommand
{
  public DeckTestingPageViewModel Viewmodel { get; } = viewmodel;

  protected override void Execute()
  {
    Viewmodel.TurnCount++;
    Viewmodel.DrawCardCommand?.Execute(null);
    Viewmodel.RaiseNewTurnStarted();
  }
}
