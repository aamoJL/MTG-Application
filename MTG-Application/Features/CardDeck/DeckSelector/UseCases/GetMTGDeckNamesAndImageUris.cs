using Microsoft.EntityFrameworkCore;
using MTGApplication.API.CardAPI;
using MTGApplication.Database;
using MTGApplication.General;
using MTGApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public class GetMTGDeckNamesAndImageUris : UseCase<Task<List<(string Name, string ImageUri)>>>
{
  public GetMTGDeckNamesAndImageUris(CardDbContextFactory contextFactory, ICardAPI<MTGCard> cardAPI)
  {
    ContextFactory = contextFactory;
    CardAPI = cardAPI;
  }

  public CardDbContextFactory ContextFactory { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public async override Task<List<(string Name, string ImageUri)>> Execute()
  {
    using var db = ContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;

    var decks = await Task.WhenAll(
      db.MTGDecks
      .Include(x => x.Commander)
      .Include(x => x.CommanderPartner)
      .Select(x => x.AsMTGCardDeck(CardAPI)));

    db.ChangeTracker.AutoDetectChangesEnabled = true;

    return decks.Select(x => (x.Name, x.Commander?.Info.FrontFace.ImageUri ?? string.Empty)).ToList();
  }
}
