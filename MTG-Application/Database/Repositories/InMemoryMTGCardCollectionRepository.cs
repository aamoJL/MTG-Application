﻿using MTGApplication.API.CardAPI;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.Database.Repositories;

/// <summary>
/// <see cref="MTGCardCollection"/> repository, that stores the items in memory
/// </summary>
public class InMemoryMTGCardCollectionRepository : IRepository<MTGCardCollection>
{
  #region Properties
  protected static List<MTGCardCollectionDTO> Collections { get; } = new();
  protected ICardAPI<MTGCard> CardAPI { get; set; }
  #endregion

  public InMemoryMTGCardCollectionRepository(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  #region IRepository implementation
  public virtual async Task<bool> Add(MTGCardCollection item)
  {
    if (!await Exists(item.Name))
    {
      Collections.Add(item.AsDTO());
      return true;
    }
    else { return false; }
  }

  public virtual async Task<bool> AddOrUpdate(MTGCardCollection item)
  {
    if (await Exists(item.Name)) { return await Update(item); }
    else { return await Add(item); }
  }

  public virtual async Task<bool> Exists(string name) => await Task.Run(() => Collections.FirstOrDefault(x => x.Name == name) != null);

  public virtual async Task<IEnumerable<MTGCardCollection>> Get(Expression<Func<MTGCardDeckDTO, object>>[] Includes = null) => await Task.WhenAll(Collections.Select(async x => await x.AsMTGCardCollection(CardAPI)));

  public virtual async Task<MTGCardCollection> Get(string name, Expression<Func<MTGCardCollection, object>>[] Includes = null) => await Collections.FirstOrDefault(x => x.Name == name)?.AsMTGCardCollection(CardAPI);

  public virtual async Task<bool> Delete(MTGCardCollection item) => await Task.Run(() => Collections.Remove(Collections.FirstOrDefault(x => x.Name == item.Name)));

  public virtual async Task<bool> Update(MTGCardCollection item)
  {
    var index = await Task.Run(() => Collections.FindIndex(x => x.Name == item.Name));
    if (index >= 0) { Collections[index] = item.AsDTO(); return true; }
    else { return false; }
  }
  #endregion
}
