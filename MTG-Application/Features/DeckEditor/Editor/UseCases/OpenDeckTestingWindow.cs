using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public class OpenDeckTestingWindow(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
{
  protected override bool CanExecute() => Viewmodel.DeckCardList.Cards.Any();

  protected override async Task Execute()
  {
    var cardsWithTokens = new List<MTGCard>([.. Viewmodel.DeckCardList.Cards
      .Where(x => x.Info.Tokens.Length > 0)]);

    if (Viewmodel.Commander != null && Viewmodel.Commander.Info.Tokens.Length > 0)
      cardsWithTokens.Add(Viewmodel.Commander);

    if (Viewmodel.Partner != null && Viewmodel.Partner.Info.Tokens.Length > 0)
      cardsWithTokens.Add(Viewmodel.Partner);

    var tokens = await Viewmodel.Worker.DoWork(GetTokens(cardsWithTokens));

    var testingDeck = new DeckTestingDeck(
      DeckCards: Viewmodel.DeckCardList.Cards.SelectMany(c => Enumerable.Repeat(c as MTGCard, c.Count)).ToList(),
      Commander: Viewmodel.Commander,
      Partner: Viewmodel.Partner,
      Tokens: tokens);

    new AppWindows.DeckTestingWindow.DeckTestingWindow(testingDeck).Activate();
  }

  private async Task<List<MTGCard>> GetTokens(List<MTGCard> cards)
  {
    var ids = string.Join(Environment.NewLine,
      cards.SelectMany(c => c.Info.Tokens.Select(t => t.ScryfallId.ToString())));

    return (await new FetchCardsWithImportString(Viewmodel.Importer).Execute(ids)).Found
      .Select(x => new MTGCard(x.Info))
      .DistinctBy(x => x.Info.OracleId)
      .ToList(); // Filter duplicates out using oracleId
  }
}