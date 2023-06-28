using Microsoft.EntityFrameworkCore;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Database.Repositories;

/// <summary>
/// <see cref="MTGCardDeck"/> repository, that stores the items in SQLite database file
/// </summary>
public class SQLiteMTGDeckRepository : IRepository<MTGCardDeck>
{
  protected readonly CardDbContextFactory cardDbContextFactory;

  public SQLiteMTGDeckRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory)
  {
    CardAPI = cardAPI;
    this.cardDbContextFactory = cardDbContextFactory;
  }

  public ICardAPI<MTGCard> CardAPI { get; init; }

  #region IRepository implementation
  public virtual async Task<bool> Add(MTGCardDeck item)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    if (!await Exists(item.Name))
    {
      db.Add(new MTGCardDeckDTO(item));
    }
    return await db.SaveChangesAsync() > 0;
  }

  public virtual async Task<bool> AddOrUpdate(MTGCardDeck item)
  {
    if (await Exists(item.Name))
    { return await Update(item); }
    else
    { return await Add(item); }
  }

  public virtual async Task<bool> Exists(string name)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    return await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == name) != null;
  }

  public virtual async Task<IEnumerable<MTGCardDeck>> Get()
  {
    using var db = cardDbContextFactory.CreateDbContext();
    var decks = await Task.WhenAll(db.MTGDecks.Select(x => x.AsMTGCardDeck(CardAPI)));
    return decks;
  }

  public virtual async Task<MTGCardDeck> Get(string name)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    var deck = db.MTGDecks.Where(x => x.Name == name)
      .Include(x => x.DeckCards)
      .Include(x => x.WishlistCards)
      .Include(x => x.MaybelistCards)
      .Include(x => x.Commander)
      .Include(x => x.CommanderPartner)
      .FirstOrDefault();
    return await deck.AsMTGCardDeck(CardAPI);
  }

  public virtual async Task<bool> Remove(MTGCardDeck item)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    var dbItem = await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == item.Name);
    if (dbItem == null)
    { return false; }
    db.Remove(dbItem);
    return await db.SaveChangesAsync() > 0;
  }

  public virtual async Task<bool> Update(MTGCardDeck item)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    if (db.MTGDecks.Where(x => x.Name == item.Name)
      .Include(x => x.DeckCards)
      .Include(x => x.WishlistCards)
      .Include(x => x.MaybelistCards)
      .Include(x => x.Commander)
      .Include(x => x.CommanderPartner)
      .FirstOrDefault() is MTGCardDeckDTO deckDTO)
    {
      // Remove unused cards from the database
      List<Guid> deckCardIds = new(item.DeckCards.Select(x => x.Info.ScryfallId).ToList());
      List<Guid> wishlistCardIds = new(item.Wishlist.Select(x => x.Info.ScryfallId).ToList());
      List<Guid> maybelistCardIds = new(item.Maybelist.Select(x => x.Info.ScryfallId).ToList());

      List<MTGCardDTO> missingCards = new();
      missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckCards.Id == deckDTO.Id && !deckCardIds.Contains(cardDTO.ScryfallId)).ToList());
      missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckWishlist.Id == deckDTO.Id && !wishlistCardIds.Contains(cardDTO.ScryfallId)).ToList());
      missingCards.AddRange(db.MTGCards.Where(cardDTO => cardDTO.DeckMaybelist.Id == deckDTO.Id && !maybelistCardIds.Contains(cardDTO.ScryfallId)).ToList());

      db.RemoveRange(missingCards);

      // Add new cards to the deckDTO
      foreach (var card in item.DeckCards)
      {
        if (deckDTO.DeckCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is MTGCardDTO cdto)
        { cdto.Count = card.Count; }
        else
        { deckDTO.DeckCards.Add(new MTGCardDTO(card)); }
      }
      foreach (var card in item.Wishlist)
      {
        if (deckDTO.WishlistCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is MTGCardDTO cdto)
        { cdto.Count = card.Count; }
        else
        { deckDTO.WishlistCards.Add(new MTGCardDTO(card)); }
      }
      foreach (var card in item.Maybelist)
      {
        if (deckDTO.MaybelistCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is MTGCardDTO cdto)
        { cdto.Count = card.Count; }
        else
        { deckDTO.MaybelistCards.Add(new MTGCardDTO(card)); }
      }

      // Remove old commander and add new one if the commander changed
      if(deckDTO.Commander?.Name != item.Commander?.Info.Name)
      {
        if(deckDTO.Commander != null)
        {
          _ = db.Remove(deckDTO.Commander);
        }
        deckDTO.Commander = item.Commander != null ? new(item.Commander) : null;
      }
      if (deckDTO.CommanderPartner?.Name != item.CommanderPartner?.Info.Name)
      {
        if(deckDTO.CommanderPartner != null)
        {
          _ = db.Remove(deckDTO.CommanderPartner);
        }
        deckDTO.CommanderPartner = item.CommanderPartner != null ? new(item.CommanderPartner) : null;
      }

      var updatedDTO = db.Update(deckDTO);
    }

    return await db.SaveChangesAsync() > 0;
  }
  #endregion
}
