using Microsoft.EntityFrameworkCore;
using MTGApplication.Database;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class DeckDTORepository : IRepository<MTGCardDeckDTO>
{
  public DeckDTORepository(CardDbContextFactory dbContextFactory = null)
    => DbContextFactory = dbContextFactory ?? new();

  public CardDbContextFactory DbContextFactory { get; }

  public async Task<bool> Add(MTGCardDeckDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();
    if (!await Exists(item.Name)) db.Add(item);
    return await db.SaveChangesAsync() > 0;
  }

  public async Task<bool> AddOrUpdate(MTGCardDeckDTO item)
    => await Exists(item.Name) ? await Update(item) : await Add(item);

  public async Task<bool> Exists(string name)
  {
    using var db = DbContextFactory.CreateDbContext();
    return await Task.FromResult(db.MTGDecks.FirstOrDefault(x => x.Name == name) != null);
  }

  public async Task<IEnumerable<MTGCardDeckDTO>> Get(Expression<Func<MTGCardDeckDTO, object>>[] Includes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var decks = db.MTGDecks.SetIncludesOrDefault(Includes);
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(decks.ToList());
  }

  // TODO: remove task?
  public async Task<MTGCardDeckDTO> Get(string name, Expression<Func<MTGCardDeckDTO, object>>[] Includes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var deck = db.MTGDecks.Where(x => x.Name == name).SetIncludesOrDefault(Includes).FirstOrDefault();
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(deck);
  }

  public async Task<bool> Delete(MTGCardDeckDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();

    if (await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == item.Name) is MTGCardDeckDTO existingItem)
      db.Remove(existingItem);

    return await db.SaveChangesAsync() > 0;
  }

  public async Task<bool> Update(MTGCardDeckDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    if (db.MTGDecks
      .Where(x => x.Name == item.Name)
      .SetIncludesOrDefault()
      .FirstOrDefault() is MTGCardDeckDTO DbDeckDTO)
    {
      db.ChangeTracker.AutoDetectChangesEnabled = true;
      // Remove unused cards from the database
      List<MTGCardDTO> missingCards = new();

      var missingCardsTasks = new List<Task>()
      {
        Task.Run(() => missingCards.AddRange(DbDeckDTO.DeckCards.Where(cardDTO => !item.DeckCards.Select(x => (name: x.Name , setCode: x.SetCode, collectorNumber: x.CollectorNumber)).Contains((name: cardDTO.Name, setCode: cardDTO.SetCode, collectorNumber: cardDTO.CollectorNumber))))),
        Task.Run(() => missingCards.AddRange(DbDeckDTO.WishlistCards.Where(cardDTO => !item.WishlistCards.Select(x => (name: x.Name , setCode: x.SetCode, collectorNumber: x.CollectorNumber)).Contains((name: cardDTO.Name, setCode: cardDTO.SetCode, collectorNumber: cardDTO.CollectorNumber))))),
        Task.Run(() => missingCards.AddRange(DbDeckDTO.MaybelistCards.Where(cardDTO => !item.MaybelistCards.Select(x =>(name: x.Name, setCode: x.SetCode, collectorNumber: x.CollectorNumber)).Contains((name: cardDTO.Name, setCode: cardDTO.SetCode, collectorNumber: cardDTO.CollectorNumber))))),
        Task.Run(() => missingCards.AddRange(DbDeckDTO.RemovelistCards.Where(cardDTO => !item.RemovelistCards.Select(x =>(name: x.Name, setCode: x.SetCode, collectorNumber: x.CollectorNumber)).Contains((name: cardDTO.Name, setCode: cardDTO.SetCode, collectorNumber: cardDTO.CollectorNumber))))),
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
            if (DbDeckDTO.DeckCards.FirstOrDefault(x => x.Name == card.Name && x.SetCode == card.SetCode && x.CollectorNumber == card.CollectorNumber) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { DbDeckDTO.DeckCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.WishlistCards)
          {
            if (DbDeckDTO.WishlistCards.FirstOrDefault(x => x.Name == card.Name && x.SetCode == card.SetCode && x.CollectorNumber == card.CollectorNumber) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { DbDeckDTO.WishlistCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.MaybelistCards)
          {
            if (DbDeckDTO.MaybelistCards.FirstOrDefault(x => x.Name == card.Name && x.SetCode == card.SetCode && x.CollectorNumber == card.CollectorNumber) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { DbDeckDTO.MaybelistCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.RemovelistCards)
          {
            if (DbDeckDTO.RemovelistCards.FirstOrDefault(x => x.Name == card.Name && x.SetCode == card.SetCode && x.CollectorNumber == card.CollectorNumber) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { DbDeckDTO.RemovelistCards.Add(card); }
          }
        })
      };

      await Task.WhenAll(updateCardsTasks);

      // Remove old commander and add new one if the commander changed
      if (DbDeckDTO.Commander?.SetCode != item.Commander?.SetCode || DbDeckDTO.Commander?.CollectorNumber != item.Commander?.CollectorNumber)
      {
        if (DbDeckDTO.Commander != null)
        {
          db.Remove(DbDeckDTO.Commander);
        }
        DbDeckDTO.Commander = item.Commander != null ? item.Commander : null;
      }
      if (DbDeckDTO.CommanderPartner?.SetCode != item.CommanderPartner?.SetCode || DbDeckDTO.CommanderPartner?.CollectorNumber != item.CommanderPartner?.CollectorNumber)
      {
        if (DbDeckDTO.CommanderPartner != null)
        {
          db.Remove(DbDeckDTO.CommanderPartner);
        }
        DbDeckDTO.CommanderPartner = item.CommanderPartner != null ? item.CommanderPartner : null;
      }

      db.Update(DbDeckDTO);
    }

    return await db.SaveChangesAsync() > 0;
  }
}
