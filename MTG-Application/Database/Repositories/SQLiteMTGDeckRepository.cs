using Microsoft.EntityFrameworkCore;
using MTGApplication.Extensions;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MTGApplication.Database.Repositories;

/// <summary>
/// <see cref="MTGCardDeck"/> repository, that stores the items in SQLite database file
/// </summary>
public class SQLiteMTGDeckRepository : IRepository<MTGCardDeck>
{
  public SQLiteMTGDeckRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory)
  {
    CardAPI = cardAPI;
    this.cardDbContextFactory = cardDbContextFactory;
  }

  protected readonly CardDbContextFactory cardDbContextFactory;

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
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var deck = db.MTGDecks.Where(x => x.Name == name).WithDefaultIncludes().FirstOrDefault();
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    
    if (deck == null) return null;
    else return await deck.AsMTGCardDeck(CardAPI);
  }

  public async Task<IEnumerable<MTGCardDeck>> GetDecksWithCommanders()
  {
    using var db = cardDbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var decks = db.MTGDecks.Include(x => x.Commander).Include(x => x.CommanderPartner).ToList();
    db.ChangeTracker.AutoDetectChangesEnabled = true;

    return await Task.WhenAll(db.MTGDecks.Select(x => x.AsMTGCardDeck(CardAPI)));
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
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    if (db.MTGDecks
      .Where(x => x.Name == item.Name)
      .WithDefaultIncludes()
      .FirstOrDefault() is MTGCardDeckDTO DbDeckDTO)
    {
      db.ChangeTracker.AutoDetectChangesEnabled = true;
      // Remove unused cards from the database
      List<MTGCardDTO> missingCards = new();

      var missingCardsTasks = new List<Task>()
      {
        Task.Run(() => missingCards.AddRange(DbDeckDTO.DeckCards.Where(cardDTO => !item.DeckCards.Select(x => (name:x.Info.Name , setCode: x.Info.SetCode, collectorNumber: x.Info.CollectorNumber)).Contains((name: cardDTO.Name, setCode: cardDTO.SetCode, collectorNumber: cardDTO.CollectorNumber))))),
        Task.Run(() => missingCards.AddRange(DbDeckDTO.WishlistCards.Where(cardDTO => !item.Wishlist.Select(x => (name:x.Info.Name , setCode: x.Info.SetCode, collectorNumber: x.Info.CollectorNumber)).Contains((name: cardDTO.Name, setCode: cardDTO.SetCode, collectorNumber: cardDTO.CollectorNumber))))),
        Task.Run(() => missingCards.AddRange(DbDeckDTO.MaybelistCards.Where(cardDTO => !item.Maybelist.Select(x =>(name : x.Info.Name, setCode : x.Info.SetCode, collectorNumber : x.Info.CollectorNumber)).Contains((name : cardDTO.Name, setCode : cardDTO.SetCode, collectorNumber : cardDTO.CollectorNumber))))),
        Task.Run(() => missingCards.AddRange(DbDeckDTO.RemovelistCards.Where(cardDTO => !item.Removelist.Select(x =>(name : x.Info.Name, setCode : x.Info.SetCode, collectorNumber : x.Info.CollectorNumber)).Contains((name : cardDTO.Name, setCode : cardDTO.SetCode, collectorNumber : cardDTO.CollectorNumber))))),
      };

      await Task.WhenAll(missingCardsTasks);

      db.RemoveRange(missingCards);

      // Add new cards to the deckDTO
      var updateCardsTasks = new List<Task>()
      {
        Task.Run(() =>
        {
          foreach (var card in item.DeckCards)
          {
            if (DbDeckDTO.DeckCards.FirstOrDefault(x => x.Name == card.Info.Name && x.SetCode == card.Info.SetCode && x.CollectorNumber == card.Info.CollectorNumber) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { DbDeckDTO.DeckCards.Add(new MTGCardDTO(card)); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.Wishlist)
          {
            if (DbDeckDTO.WishlistCards.FirstOrDefault(x => x.Name == card.Info.Name && x.SetCode == card.Info.SetCode && x.CollectorNumber == card.Info.CollectorNumber) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { DbDeckDTO.WishlistCards.Add(new MTGCardDTO(card)); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.Maybelist)
          {
            if (DbDeckDTO.MaybelistCards.FirstOrDefault(x => x.Name == card.Info.Name && x.SetCode == card.Info.SetCode && x.CollectorNumber == card.Info.CollectorNumber) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { DbDeckDTO.MaybelistCards.Add(new MTGCardDTO(card)); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.Removelist)
          {
            if (DbDeckDTO.RemovelistCards.FirstOrDefault(x => x.Name == card.Info.Name && x.SetCode == card.Info.SetCode && x.CollectorNumber == card.Info.CollectorNumber) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { DbDeckDTO.RemovelistCards.Add(new MTGCardDTO(card)); }
          }
        })
      };

      await Task.WhenAll(updateCardsTasks);

      // Remove old commander and add new one if the commander changed
      if (DbDeckDTO.Commander?.SetCode != item.Commander?.Info.SetCode || DbDeckDTO.Commander?.CollectorNumber != item.Commander?.Info.CollectorNumber)
      {
        if (DbDeckDTO.Commander != null)
        {
          db.Remove(DbDeckDTO.Commander);
        }
        DbDeckDTO.Commander = item.Commander != null ? new(item.Commander) : null;
      }
      if (DbDeckDTO.CommanderPartner?.SetCode != item.CommanderPartner?.Info.SetCode || DbDeckDTO.CommanderPartner?.CollectorNumber != item.CommanderPartner?.Info.CollectorNumber)
      {
        if (DbDeckDTO.CommanderPartner != null)
        {
          db.Remove(DbDeckDTO.CommanderPartner);
        }
        DbDeckDTO.CommanderPartner = item.CommanderPartner != null ? new(item.CommanderPartner) : null;
      }

      db.Update(DbDeckDTO);
    }

    return await db.SaveChangesAsync() > 0;
  }
  #endregion
}
