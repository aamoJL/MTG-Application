using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckTesting.ViewModels;
public partial class DeckTestingPageViewModel
{
  public class Factory(MTGCardImporter importer)
  {
    public DeckTestingPageViewModel Build(DeckTestingDeck deck)
    {
      var viewmodel = new DeckTestingPageViewModel(deck);

      _ = UpdateTokens(viewmodel, deck);

      return viewmodel;
    }

    private async Task UpdateTokens(DeckTestingPageViewModel viewmodel, DeckTestingDeck deck)
    {
      var cardsWithTokens = new List<MTGCard>([.. deck.DeckCards.Where(x => x.Info.Tokens.Length > 0)]);

      if (deck.Commander != null && deck.Commander.Info.Tokens.Length > 0)
        cardsWithTokens.Add(deck.Commander);

      if (deck.Partner != null && deck.Partner.Info.Tokens.Length > 0)
        cardsWithTokens.Add(deck.Partner);

      var tokens = await GetTokens(cardsWithTokens);

      foreach (var item in tokens)
        viewmodel.Tokens.Add(item);
    }

    private async Task<List<DeckTestingMTGCard>> GetTokens(List<MTGCard> cards)
    {
      var ids = string.Join(Environment.NewLine,
        cards.SelectMany(c => c.Info.Tokens.Select(t => t.ScryfallId.ToString())));

      return (await new FetchCardsWithImportString(importer).Execute(ids)).Found
        .Select(x => new DeckTestingMTGCard(x.Info) { IsToken = true })
        .DistinctBy(x => x.Info.OracleId)
        .ToList(); // Filter duplicates out using oracleId
    }
  }
}

