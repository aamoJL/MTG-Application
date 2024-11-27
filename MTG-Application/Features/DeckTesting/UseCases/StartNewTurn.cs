using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckTesting.UseCases;

public class StartNewTurn(DeckTestingPageViewModel viewmodel) : ViewModelCommand<DeckTestingPageViewModel>(viewmodel)
{
  protected override void Execute()
  {
    Viewmodel.TurnCount++;
    Viewmodel.DrawCardCommand?.Execute(null);
    Viewmodel.RaiseNewTurnStarted();
  }
}
