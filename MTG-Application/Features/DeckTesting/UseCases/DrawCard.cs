using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.General.ViewModels;
using System;

namespace MTGApplication.Features.DeckTesting.UseCases;

public class DrawCard(DeckTestingPageViewModel viewmodel) : ViewModelCommand<DeckTestingPageViewModel>(viewmodel)
{
  protected override void Execute()
  {
  }
}
