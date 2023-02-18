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
    protected readonly CardDbContextFactory cardDbContextFactory;

    public SQLiteMTGDeckRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory)
    {
      CardAPI = cardAPI;
      this.cardDbContextFactory = cardDbContextFactory;
    }

    public ICardAPI<MTGCard> CardAPI { get; init; }

    public virtual async Task<bool> Add(MTGCardDeck deck)
    {
      using var db = cardDbContextFactory.CreateDbContext();
      if (!await Exists(deck.Name))
      {
        db.Add(new MTGCardDeckDTO(deck));
      }
      return await db.SaveChangesAsync() > 0;
    }
    public virtual async Task<bool> AddOrUpdate(MTGCardDeck deck)
    {
      if (await Exists(deck.Name)) { return await Update(deck); }
      else { return await Add(deck); }
    }
    public virtual async Task<bool> Exists(string name)
    {
      using var db = cardDbContextFactory.CreateDbContext();
      return await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == name) != null;
    }
    public virtual async Task<IEnumerable<MTGCardDeck>> Get()
    {
      using var db = cardDbContextFactory.CreateDbContext();
      var decks = await Task.WhenAll(db.MTGDecks.Select(x => MTGCardDeckDTOConverter.Convert(x, CardAPI)));
      return decks;
    }
    public virtual async Task<MTGCardDeck> Get(string name)
    {
      using var db = cardDbContextFactory.CreateDbContext();
      var deck = db.MTGDecks.Where(x => x.Name == name).Include(x => x.DeckCards).Include(x => x.WishlistCards).Include(x => x.MaybelistCards).FirstOrDefault();

      return await MTGCardDeckDTOConverter.Convert(deck, CardAPI);
    }
    public virtual async Task<bool> Remove(MTGCardDeck deck)
    {
      using var db = cardDbContextFactory.CreateDbContext();
      db.Remove(await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == deck.Name));
      return await db.SaveChangesAsync() > 0;
    }
    public virtual async Task<bool> Update(MTGCardDeck deck)
    {
      using var db = cardDbContextFactory.CreateDbContext();
      if (await db.MTGDecks.Where(x => x.Name == deck.Name).Include(x => x.DeckCards).Include(x => x.WishlistCards).Include(x => x.MaybelistCards).FirstOrDefaultAsync() is MTGCardDeckDTO deckDTO)
      {
        // Remove unused cards from the database
        List<Guid> deckCardIds = new(deck.DeckCards.Select(x => x.Info.ScryfallId).ToList());
        List<Guid> wishlistCardIds = new(deck.Wishlist.Select(x => x.Info.ScryfallId).ToList());
        List<Guid> maybelistCardIds = new(deck.Maybelist.Select(x => x.Info.ScryfallId).ToList());

        List<CardDTO> missingCards = new();
        missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckCards.Id == deckDTO.Id && !deckCardIds.Contains(cardDTO.ScryfallId)).ToList());
        missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckWishlist.Id == deckDTO.Id && !wishlistCardIds.Contains(cardDTO.ScryfallId)).ToList());
        missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckMaybelist.Id == deckDTO.Id && !maybelistCardIds.Contains(cardDTO.ScryfallId)).ToList());

        db.RemoveRange(missingCards);

        // Add new cards to the deckDTO
        foreach (var card in deck.DeckCards)
        {
          if(deckDTO.DeckCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is CardDTO cdto) { cdto.Count = card.Count; }
          else { deckDTO.DeckCards.Add(new CardDTO(card)); }
        }
        foreach (var card in deck.Wishlist)
        {
          if (deckDTO.WishlistCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is CardDTO cdto) { cdto.Count = card.Count; }
          else { deckDTO.WishlistCards.Add(new CardDTO(card)); }
        }
        foreach (var card in deck.Maybelist)
        {
          if (deckDTO.MaybelistCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is CardDTO cdto) { cdto.Count = card.Count; }
          else { deckDTO.MaybelistCards.Add(new CardDTO(card)); }
        }

        var updatedDTO = db.Update(deckDTO);
      }

      return await db.SaveChangesAsync() > 0;
    }
  }
}
