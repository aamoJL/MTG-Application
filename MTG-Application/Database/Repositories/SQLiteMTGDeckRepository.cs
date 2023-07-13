using Microsoft.EntityFrameworkCore;
using MTGApplication.Extensions;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
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
    if (await Exists(item.Name)) { return await Update(item); }
    else { return await Add(item); }
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
    var deck = db.MTGDecks
      .WithDefaultIncludes()
      .Where(x => x.Name == name)
      .FirstOrDefault();
    return await deck.AsMTGCardDeck(CardAPI);
  }

  public virtual async Task<bool> Remove(MTGCardDeck item)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    var dbItem = await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == item.Name);
    if (dbItem == null) { return false; }
    db.Remove(dbItem);
    return await db.SaveChangesAsync() > 0;
  }

  public virtual async Task<bool> Update(MTGCardDeck item)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    if (db.MTGDecks
      .WithDefaultIncludes()
      .Where(x => x.Name == item.Name)
      .FirstOrDefault() is MTGCardDeckDTO DbDeckDTO)
    {
      // Remove unused cards from the database
      List<MTGCardDTO> missingCards = new();
      missingCards.AddRange(DbDeckDTO.DeckCards.Where(cardDTO => !item.DeckCards.Select(x => x.Info.ScryfallId).ToList().Contains(cardDTO.ScryfallId)).ToList());
      missingCards.AddRange(DbDeckDTO.WishlistCards.Where(cardDTO => !item.Wishlist.Select(x => x.Info.ScryfallId).ToList().Contains(cardDTO.ScryfallId)).ToList());
      missingCards.AddRange(DbDeckDTO.MaybelistCards.Where(cardDTO => !item.Maybelist.Select(x => x.Info.ScryfallId).ToList().Contains(cardDTO.ScryfallId)).ToList());
      missingCards.AddRange(DbDeckDTO.RemovelistCards.Where(cardDTO => !item.Removelist.Select(x => x.Info.ScryfallId).ToList().Contains(cardDTO.ScryfallId)).ToList());

      db.RemoveRange(missingCards);

      // Add new cards to the deckDTO
      foreach (var card in item.DeckCards)
      {
        if (DbDeckDTO.DeckCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is MTGCardDTO cdto) { cdto.Count = card.Count; }
        else { DbDeckDTO.DeckCards.Add(new MTGCardDTO(card)); }
      }
      foreach (var card in item.Wishlist)
      {
        if (DbDeckDTO.WishlistCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is MTGCardDTO cdto) { cdto.Count = card.Count; }
        else { DbDeckDTO.WishlistCards.Add(new MTGCardDTO(card)); }
      }
      foreach (var card in item.Maybelist)
      {
        if (DbDeckDTO.MaybelistCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is MTGCardDTO cdto) { cdto.Count = card.Count; }
        else { DbDeckDTO.MaybelistCards.Add(new MTGCardDTO(card)); }
      }
      foreach (var card in item.Removelist)
      {
        if (DbDeckDTO.RemovelistCards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is MTGCardDTO cdto) { cdto.Count = card.Count; }
        else { DbDeckDTO.RemovelistCards.Add(new MTGCardDTO(card)); }
      }

      // Remove old commander and add new one if the commander changed
      if (DbDeckDTO.Commander?.Name != item.Commander?.Info.Name)
      {
        if (DbDeckDTO.Commander != null)
        {
          _ = db.Remove(DbDeckDTO.Commander);
        }
        DbDeckDTO.Commander = item.Commander != null ? new(item.Commander) : null;
      }
      if (DbDeckDTO.CommanderPartner?.Name != item.CommanderPartner?.Info.Name)
      {
        if (DbDeckDTO.CommanderPartner != null)
        {
          _ = db.Remove(DbDeckDTO.CommanderPartner);
        }
        DbDeckDTO.CommanderPartner = item.CommanderPartner != null ? new(item.CommanderPartner) : null;
      }

      var updatedDTO = db.Update(DbDeckDTO);
    }

    return await db.SaveChangesAsync() > 0;
  }
  #endregion
}
