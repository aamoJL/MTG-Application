using Microsoft.EntityFrameworkCore;
using MTGApplication.API.CardAPI;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Database.Repositories;

/// <summary>
/// <see cref="MTGCardCollection"/> repository, that stores the items in a SQLite database file
/// </summary>
public class SQLiteMTGCardCollectionRepository : IRepository<MTGCardCollection>
{
  public SQLiteMTGCardCollectionRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory)
  {
    CardAPI = cardAPI;
    this.cardDbContextFactory = cardDbContextFactory;
  }

  protected readonly CardDbContextFactory cardDbContextFactory;

  public ICardAPI<MTGCard> CardAPI { get; init; }

  #region IRepository Implementation
  public async Task<bool> Add(MTGCardCollection item)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    if (!await Exists(item.Name))
    {
      db.Add(item.AsDTO());
    }
    return await db.SaveChangesAsync() > 0;
  }

  public async Task<bool> AddOrUpdate(MTGCardCollection item)
  {
    if (await Exists(item.Name)) { return await Update(item); }
    else { return await Add(item); }
  }

  public async Task<bool> Exists(string name)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    return await db.MTGCardCollections.FirstOrDefaultAsync(x => x.Name == name) != null;
  }

  public async Task<IEnumerable<MTGCardCollection>> Get()
  {
    using var db = cardDbContextFactory.CreateDbContext();
    return await Task.WhenAll(db.MTGCardCollections.Select(x => x.AsMTGCardCollection(CardAPI)));
  }

  public async Task<MTGCardCollection> Get(string name)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    var collection = await db.MTGCardCollections.Where(x => x.Name == name).Include(x => x.CollectionLists).ThenInclude(x => x.Cards).FirstOrDefaultAsync();
    return await collection.AsMTGCardCollection(CardAPI);
  }

  public async Task<bool> Remove(MTGCardCollection item)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    var dbItem = await db.MTGCardCollections.FirstOrDefaultAsync(x => x.Name == item.Name);
    if (dbItem == null) { return false; }
    db.Remove(dbItem);
    return await db.SaveChangesAsync() > 0;
  }

  public async Task<bool> Update(MTGCardCollection item)
  {
    using var db = cardDbContextFactory.CreateDbContext();
    if (await db.MTGCardCollections.Where(x => x.Name == item.Name).Include(x => x.CollectionLists).ThenInclude(x => x.Cards).FirstOrDefaultAsync() is MTGCardCollectionDTO dbCollectionDTO)
    {
      // Remove unused card lists from the database
      foreach (var listDTO in dbCollectionDTO.CollectionLists)
      {
        if (item.CollectionLists.FirstOrDefault(x => x.Name == listDTO.Name) is null)
        {
          db.MTGCardCollectionLists.Remove(listDTO);
        }
      }

      foreach (var itemList in item.CollectionLists)
      {
        // Find item list from the database, add if it does not exist
        var dbListDTO = dbCollectionDTO.CollectionLists.FirstOrDefault(x => x.Name == itemList.Name);
        if (dbListDTO == null)
        {
          dbCollectionDTO.CollectionLists.Add(new(itemList));
          continue;
        }
        else
        {
          // Update list's search query
          dbListDTO.SearchQuery = itemList.SearchQuery;

          // Remove unused cards from the database
          List<MTGCardDTO> missingCards = new();
          missingCards.AddRange(dbListDTO.Cards.Where(cardDTO => !itemList.Cards.Select(x => x.Info.ScryfallId).ToList().Contains(cardDTO.ScryfallId)).ToList());
          db.RemoveRange(missingCards);

          // Add new cards to the deckDTO
          foreach (var card in itemList.Cards)
          {
            if (dbListDTO.Cards.FirstOrDefault(x => x.ScryfallId == card.Info.ScryfallId) is MTGCardDTO cdto)
            { cdto.Count = card.Count; }
            else
            { dbListDTO.Cards.Add(new(card)); }
          }
        }
      }

      db.Update(dbCollectionDTO);
    }

    return await db.SaveChangesAsync() > 0;
  }
  #endregion
}
