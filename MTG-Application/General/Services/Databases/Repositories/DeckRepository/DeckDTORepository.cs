using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Context;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.DeckRepository;

public class DeckDTORepository(CardDbContextFactory? dbContextFactory = null) : IRepository<MTGCardDeckDTO>
{
  public CardDbContextFactory DbContextFactory { get; } = dbContextFactory ?? new();

  public virtual async Task<bool> Add(MTGCardDeckDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();

    if (!await Exists(item.Name))
      db.Add(item);

    return await db.SaveChangesAsync() > 0;
  }

  public virtual async Task<bool> AddOrUpdate(MTGCardDeckDTO item)
    => await Exists(item.Name) ? await Update(item) : await Add(item);

  public virtual async Task<bool> Exists(string name)
  {
    using var db = DbContextFactory.CreateDbContext();
    return await Task.FromResult(db.MTGDecks?.FirstOrDefault(x => x.Name == name) != null);
  }

  public virtual async Task<IEnumerable<MTGCardDeckDTO>> Get(Action<DbSet<MTGCardDeckDTO>>? setIncludes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;

    if (db.MTGDecks is DbSet<MTGCardDeckDTO> set)
    {
      if (setIncludes != null)
        setIncludes.Invoke(set);
      else
        SetDefaultIncludes(set);

      var items = set.ToList();
      db.ChangeTracker.AutoDetectChangesEnabled = true;

      return await Task.FromResult(items);
    }

    return [];
  }

  public virtual async Task<MTGCardDeckDTO?> Get(string name, Action<DbSet<MTGCardDeckDTO>>? setIncludes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;

    if (db.MTGDecks is not DbSet<MTGCardDeckDTO> set)
      return null;

    if (setIncludes != null)
      setIncludes.Invoke(set);
    else
      SetDefaultIncludes(set);

    var deck = set?.Where(x => x.Name == name).FirstOrDefault();
    db.ChangeTracker.AutoDetectChangesEnabled = true;

    return await Task.FromResult(deck);
  }

  public virtual async Task<bool> Delete(MTGCardDeckDTO item)
  {
    if (item == null) return false;

    using var db = DbContextFactory.CreateDbContext();
    var set = db.MTGDecks;

    if (set != null && await set.FirstOrDefaultAsync(x => x.Name == item.Name) is MTGCardDeckDTO existingItem)
      db.Remove(existingItem);

    return await db.SaveChangesAsync() > 0;
  }

  /// <exception cref="ArgumentNullException"></exception>
  public virtual async Task<bool> Update(MTGCardDeckDTO item)
  {
    ArgumentNullException.ThrowIfNull(item);

    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;

    if (await Get(item.Name) is not MTGCardDeckDTO existingDeck)
      return false;

    db.ChangeTracker.AutoDetectChangesEnabled = true;

    // Remove unused cards from the database
    List<MTGCardDTO> missingDeckCards = [];
    List<MTGCardDTO> missingWishlistCards = [];
    List<MTGCardDTO> missingMaybelistCards = [];
    List<MTGCardDTO> missingRemovelistCards = [];

    await Task.WhenAll(new List<Task>()
      {
        Task.Run(() => missingDeckCards.AddRange(existingDeck.DeckCards.Where(existingCard => !item.DeckCards.Any(itemCard => itemCard.Compare(existingCard))))),
        Task.Run(() => missingWishlistCards.AddRange(existingDeck.WishlistCards.Where(existingCard => !item.WishlistCards.Any(itemCard => itemCard.Compare(existingCard))))),
        Task.Run(() => missingMaybelistCards.AddRange(existingDeck.MaybelistCards.Where(existingCard => !item.MaybelistCards.Any(itemCard => itemCard.Compare(existingCard))))),
        Task.Run(() => missingRemovelistCards.AddRange(existingDeck.RemovelistCards.Where(existingCard => !item.RemovelistCards.Any(itemCard => itemCard.Compare(existingCard))))),
      });

    db.RemoveRange([.. missingDeckCards, .. missingWishlistCards, .. missingMaybelistCards, .. missingRemovelistCards]);

    // Add new cards to the deckDTO
    await Task.WhenAll(new List<Task>()
      {
        Task.Run(() =>
        {
          foreach (var card in item.DeckCards)
          {
            if(existingDeck.DeckCards.FindIndex(x => x.Compare(card)) is var i && i >= 0)
            {
              var existingCard = existingDeck.DeckCards[i];
              existingDeck.DeckCards[i] = card with {Id = existingCard.Id};
            }
            else { existingDeck.DeckCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.MaybelistCards)
          {
            if(existingDeck.MaybelistCards.FindIndex(x => x.Compare(card)) is var i && i >= 0)
            {
              var existingCard = existingDeck.MaybelistCards[i];
              existingDeck.MaybelistCards[i] = card with {Id = existingCard.Id};
            }
            else { existingDeck.MaybelistCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.WishlistCards)
          {
            if(existingDeck.WishlistCards.FindIndex(x => x.Compare(card)) is var i && i >= 0)
            {
              var existingCard = existingDeck.WishlistCards[i];
              existingDeck.WishlistCards[i] = card with {Id = existingCard.Id};
            }
            else { existingDeck.WishlistCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.RemovelistCards)
          {
            if(existingDeck.RemovelistCards.FindIndex(x => x.Compare(card)) is var i && i >= 0)
            {
              var existingCard = existingDeck.RemovelistCards[i];
              existingDeck.RemovelistCards[i] = card with {Id = existingCard.Id};
            }
            else { existingDeck.RemovelistCards.Add(card); }
          }
        })
      });

    // Remove old commander and add new one if the commander changed
    if ((existingDeck.Commander != null || item.Commander != null) && existingDeck.Commander?.Compare(item.Commander) is not true)
    {
      if (existingDeck.Commander != null)
        db.Remove(existingDeck.Commander);

      existingDeck.Commander = item?.Commander ?? null;
    }

    if ((existingDeck.CommanderPartner != null || item?.CommanderPartner != null) && existingDeck.CommanderPartner?.Compare(item?.CommanderPartner) is not true)
    {
      if (existingDeck.CommanderPartner != null)
        db.Remove(existingDeck.CommanderPartner);

      existingDeck.CommanderPartner = item?.CommanderPartner ?? null;
    }

    db.Update(existingDeck);

    return await db.SaveChangesAsync() > 0;
  }

  protected static void SetDefaultIncludes(DbSet<MTGCardDeckDTO> db)
  {
    db.Include(x => x.DeckCards).Load();
    db.Include(x => x.WishlistCards).Load();
    db.Include(x => x.MaybelistCards).Load();
    db.Include(x => x.RemovelistCards).Load();
    db.Include(x => x.Commander).Load();
    db.Include(x => x.CommanderPartner).Load();
  }
}
