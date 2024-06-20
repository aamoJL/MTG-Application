using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.General.ViewModels;
using System;

namespace MTGApplication.Features.DeckTesting.UseCases;

public class StartNewTurn(DeckTestingPageViewModel viewmodel) : ViewModelCommand<DeckTestingPageViewModel>(viewmodel)
{
  protected override void Execute()
  {
    throw new NotImplementedException();
  }
}
