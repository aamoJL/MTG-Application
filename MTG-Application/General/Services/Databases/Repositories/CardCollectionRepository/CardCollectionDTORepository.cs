using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Context;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
public class CardCollectionDTORepository(CardDbContextFactory dbContextFactory = null) : IRepository<MTGCardCollectionDTO>
{
  public CardDbContextFactory DbContextFactory { get; } = dbContextFactory ?? new();

  public virtual async Task<bool> Add(MTGCardCollectionDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();
    if (!await Exists(item.Name)) db.Add(item);
    return await db.SaveChangesAsync() > 0;
  }

  public virtual async Task<bool> AddOrUpdate(MTGCardCollectionDTO item)
    => await Exists(item.Name) ? await Update(item) : await Add(item);

  public virtual async Task<bool> Delete(MTGCardCollectionDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();

    if (await db.MTGCardCollections.FirstOrDefaultAsync(x => x.Name == item.Name) is not MTGCardCollectionDTO existingItem)
      return false;

    db.Remove(existingItem);
    return await db.SaveChangesAsync() > 0;
  }

  public virtual async Task<bool> Exists(string name)
  {
    using var db = DbContextFactory.CreateDbContext();
    return await Task.FromResult(db.MTGCardCollections.FirstOrDefault(x => x.Name == name) != null);
  }

  public virtual async Task<IEnumerable<MTGCardCollectionDTO>> Get(Action<DbSet<MTGCardCollectionDTO>> setIncludes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;

    var set = db.MTGCardCollections;

    if (setIncludes != null) setIncludes.Invoke(set);
    else SetDefaultIncludes(set);

    var items = set.ToList();
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(items);
  }

  public virtual async Task<MTGCardCollectionDTO> Get(string name, Action<DbSet<MTGCardCollectionDTO>> setIncludes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;

    var set = db.MTGCardCollections;

    if (setIncludes != null) setIncludes.Invoke(set);
    else SetDefaultIncludes(set);

    var item = set.Where(x => x.Name == name).FirstOrDefault();
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(item);
  }

  public virtual async Task<bool> Update(MTGCardCollectionDTO item)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;

    if (await Get(item.Name) is not MTGCardCollectionDTO existingCollection)
      return false;

    // Remove unused card lists from the database
    foreach (var listDTO in existingCollection.CollectionLists)
    {
      if (item.CollectionLists.FirstOrDefault(x => x.Name == listDTO.Name) is null)
        db.MTGCardCollectionLists.Remove(listDTO);
    }

    foreach (var itemList in item.CollectionLists)
    {
      // Find item list from the database, add if it does not exist
      if (existingCollection.CollectionLists.FirstOrDefault(x => x.Name == itemList.Name)
        is not MTGCardCollectionListDTO existingList)
      {
        existingCollection.CollectionLists.Add(itemList);
      }
      else
      {
        // Update list's search query
        existingList.SearchQuery = itemList.SearchQuery;

        // Remove unused cards from the database
        db.RemoveRange([.. existingList.Cards
            .Where(cardDTO => !itemList.Cards.Select(x => x.ScryfallId).Contains(cardDTO.ScryfallId))]);

        // Add or update new cards to the deckDTO
        foreach (var card in itemList.Cards)
        {
          if (existingList.Cards.FirstOrDefault(x => x.ScryfallId == card.ScryfallId) is MTGCardDTO existingCard)
            existingCard.Count = card.Count;
          else
            existingList.Cards.Add(card);
        }
      }
    }

    db.Update(existingCollection);

    return await db.SaveChangesAsync() > 0;
  }

  protected static void SetDefaultIncludes(DbSet<MTGCardCollectionDTO> db)
    => db.Include(x => x.CollectionLists).ThenInclude(x => x.Cards).Load();
}
