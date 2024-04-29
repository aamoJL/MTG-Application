using Microsoft.EntityFrameworkCore;
using MTGApplication.Database;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.DeckRepository;

public class DeckDTORepository : IRepository<MTGCardDeckDTO>
{
  public DeckDTORepository(CardDbContextFactory dbContextFactory = null)
    => DbContextFactory = dbContextFactory ?? new();

  public CardDbContextFactory DbContextFactory { get; }

  public virtual async Task<bool> Add(MTGCardDeckDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();
    if (!await Exists(item.Name)) db.Add(item);
    return await db.SaveChangesAsync() > 0;
  }

  public virtual async Task<bool> AddOrUpdate(MTGCardDeckDTO item)
    => await Exists(item.Name) ? await Update(item) : await Add(item);

  public virtual async Task<bool> Exists(string name)
  {
    using var db = DbContextFactory.CreateDbContext();
    return await Task.FromResult(db.MTGDecks.FirstOrDefault(x => x.Name == name) != null);
  }

  public virtual async Task<IEnumerable<MTGCardDeckDTO>> Get(Expression<Func<MTGCardDeckDTO, object>>[] Includes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var decks = db.MTGDecks.SetIncludesOrDefault(Includes);
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(decks.ToList());
  }

  public virtual async Task<MTGCardDeckDTO> Get(string name, Expression<Func<MTGCardDeckDTO, object>>[] Includes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var deck = db.MTGDecks.Where(x => x.Name == name).SetIncludesOrDefault(Includes).FirstOrDefault();
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(deck);
  }

  public virtual async Task<bool> Delete(MTGCardDeckDTO item)
  {
    if (item == null) return false;

    using var db = DbContextFactory.CreateDbContext();

    if (await db.MTGDecks.FirstOrDefaultAsync(x => x.Name == item.Name) is MTGCardDeckDTO existingItem)
      db.Remove(existingItem);

    return await db.SaveChangesAsync() > 0;
  }

  public virtual async Task<bool> Update(MTGCardDeckDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    if (db.MTGDecks
      .Where(x => x.Name == item.Name)
      .SetIncludesOrDefault()
      .FirstOrDefault() is MTGCardDeckDTO existingDeck)
    {
      db.ChangeTracker.AutoDetectChangesEnabled = true;
      // Remove unused cards from the database
      List<MTGCardDTO> missingDeckCards = new();
      List<MTGCardDTO> missingWishlistCards = new();
      List<MTGCardDTO> missingMaybelistCards = new();
      List<MTGCardDTO> missingRemovelistCards = new();

      var missingCardsTasks = new List<Task>()
      {
        Task.Run(() => missingDeckCards.AddRange(existingDeck.DeckCards.Where(existingCard => !item.DeckCards.Any(itemCard => itemCard.Compare(existingCard))))),
        Task.Run(() => missingWishlistCards.AddRange(existingDeck.WishlistCards.Where(existingCard => !item.DeckCards.Any(itemCard => itemCard.Compare(existingCard))))),
        Task.Run(() => missingMaybelistCards.AddRange(existingDeck.MaybelistCards.Where(existingCard => !item.DeckCards.Any(itemCard => itemCard.Compare(existingCard))))),
        Task.Run(() => missingRemovelistCards.AddRange(existingDeck.RemovelistCards.Where(existingCard => !item.DeckCards.Any(itemCard => itemCard.Compare(existingCard))))),
      };

      await Task.WhenAll(missingCardsTasks);

      db.RemoveRange(missingDeckCards);
      db.RemoveRange(missingWishlistCards);
      db.RemoveRange(missingMaybelistCards);
      db.RemoveRange(missingRemovelistCards);

      // Add new cards to the deckDTO
      var updateCardsTasks = new List<Task>()
      {
        Task.Run(() =>
        {
          foreach (var card in item.DeckCards)
          {
            if (existingDeck.DeckCards.FirstOrDefault(x => x.Compare(card)) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { existingDeck.DeckCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.WishlistCards)
          {
            if (existingDeck.WishlistCards.FirstOrDefault(x => x.Compare(card)) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { existingDeck.WishlistCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.MaybelistCards)
          {
            if (existingDeck.MaybelistCards.FirstOrDefault(x => x.Compare(card)) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { existingDeck.MaybelistCards.Add(card); }
          }
        }),
        Task.Run(() =>
        {
          foreach (var card in item.RemovelistCards)
          {
            if (existingDeck.RemovelistCards.FirstOrDefault(x => x.Compare(card)) is MTGCardDTO cdto) { cdto.Count = card.Count; }
            else { existingDeck.RemovelistCards.Add(card); }
          }
        })
      };

      // Remove old commander and add new one if the commander changed
      if ((existingDeck.Commander != null || item.Commander != null) && existingDeck.Commander?.Compare(item?.Commander) is not true)
      {
        if (existingDeck.Commander != null)
          db.Remove(existingDeck.Commander);

        existingDeck.Commander = item.Commander ?? null;
      }

      if ((existingDeck.CommanderPartner != null || item.CommanderPartner != null) && existingDeck.CommanderPartner?.Compare(item?.CommanderPartner) is not true)
      {
        if (existingDeck.CommanderPartner != null)
          db.Remove(existingDeck.CommanderPartner);

        existingDeck.CommanderPartner = item.CommanderPartner ?? null;
      }

      db.Update(existingDeck);
    }

    return await db.SaveChangesAsync() > 0;
  }
}
