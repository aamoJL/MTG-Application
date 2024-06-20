using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckTesting.UseCases;
public class StartNewGame(DeckTestingPageViewModel viewmodel) : ViewModelCommand<DeckTestingPageViewModel>(viewmodel)
{
  protected override void Execute()
  {
    Viewmodel.Library.Clear();
    Viewmodel.Graveyard.Clear();
    Viewmodel.Exile.Clear();
    Viewmodel.Hand.Clear();
    Viewmodel.CommandZone.Clear();

    Viewmodel.TurnCount = 0;
    Viewmodel.PlayerHP = 40;
    Viewmodel.EnemyHP = 40;

    // Reset library
    foreach (var item in Viewmodel.Deck.DeckCards)
      Viewmodel.Library.Add(new(item.Info));

    Viewmodel.ShuffleDeckCommand.Execute(null);

    // Add commanders to the command zone
    if (Viewmodel.Deck.Commander != null) { Viewmodel.CommandZone.Add(new(Viewmodel.Deck.Commander.Info)); }
    if (Viewmodel.Deck.Partner != null) { Viewmodel.CommandZone.Add(new(Viewmodel.Deck.Partner.Info)); }

    for (var i = 0; i < 7; i++)
      Viewmodel.DrawCardCommand.Execute(null); // Draw 7 cards from library to hand

    Viewmodel.StartNewGame();
  }
}