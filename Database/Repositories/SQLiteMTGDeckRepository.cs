using Microsoft.EntityFrameworkCore;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Models.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Database.Repositories
{
  public class SQLiteMTGDeckRepository : IDeckRepository<MTGCardDeck>
  {
    public SQLiteMTGDeckRepository(ICardAPI<MTGCard> cardAPI)
    {
      CardAPI = cardAPI;
    }

    public ICardAPI<MTGCard> CardAPI { get; init; }

    public async Task<bool> Add(MTGCardDeck deck)
    {
      using var db = new CardDbContext();
      if (!await Exists(deck.Name))
      {
        db.Add(new MTGCardDeckDTO(deck));
      }
      return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddOrUpdate(MTGCardDeck deck)
    {
      if (await Exists(deck.Name)) { return await Update(deck); }
      else { return await Add(deck); }
    }

    public async Task<bool> Exists(string name)
    {
      using var db = new CardDbContext();
      return await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == name) != null;
    }

    public async Task<IEnumerable<MTGCardDeck>> Get()
    {
      using var db = new CardDbContext();
      var decks = await Task.WhenAll(db.MTGDecks.Select(x => MTGCardDeckDTOConverter.Convert(x, CardAPI)));
      return decks;
    }

    public async Task<MTGCardDeck> Get(string name)
    {
      using var db = new CardDbContext();
      return await MTGCardDeckDTOConverter.Convert(db.MTGDecks.FirstOrDefault(x => x.Name == name), CardAPI);
    }

    public async Task<bool> Remove(MTGCardDeck deck)
    {
      using var db = new CardDbContext();
      db.Remove(await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == deck.Name));
      return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> Update(MTGCardDeck deck)
    {
      using var db = new CardDbContext();
      if (await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == deck.Name) is MTGCardDeckDTO deckDTO)
      {
        // Remove unused cards from the database
        List<Guid> validCardIds = new();
        validCardIds.AddRange(deck.DeckCards.Select(x => x.Info.ScryfallId).ToList());
        validCardIds.AddRange(deck.Wishlist.Select(x => x.Info.ScryfallId).ToList());
        validCardIds.AddRange(deck.Maybelist.Select(x => x.Info.ScryfallId).ToList());

        List<CardDTO> missingCards = new();
        missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckCards.Id == deckDTO.Id && !validCardIds.Contains(cardDTO.ScryfallId)).ToList());
        missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckWishlist.Id == deckDTO.Id && !validCardIds.Contains(cardDTO.ScryfallId)).ToList());
        missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckMaybelist.Id == deckDTO.Id && !validCardIds.Contains(cardDTO.ScryfallId)).ToList());

        db.RemoveRange(missingCards);

        // Add new cards to the deckDTO
        deckDTO.DeckCards.AddRange(deck.DeckCards.Select(dc 
          => deckDTO.DeckCards.FirstOrDefault(dtoc => dtoc.ScryfallId == dc.Info.ScryfallId) != null ? new CardDTO(dc) : null));
        deckDTO.WishlistCards.AddRange(deck.Wishlist.Select(dc
          => deckDTO.WishlistCards.FirstOrDefault(dtoc => dtoc.ScryfallId == dc.Info.ScryfallId) != null ? new CardDTO(dc) : null));
        deckDTO.MaybelistCards.AddRange(deck.Maybelist.Select(dc
          => deckDTO.MaybelistCards.FirstOrDefault(dtoc => dtoc.ScryfallId == dc.Info.ScryfallId) != null ? new CardDTO(dc) : null));

        db.Update(deckDTO);
      }

      return await db.SaveChangesAsync() > 0;
    }
  }
}
