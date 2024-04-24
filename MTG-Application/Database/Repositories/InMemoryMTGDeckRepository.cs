using MTGApplication.API.CardAPI;
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
/// <see cref="MTGCardDeck"/> repository, that stores the items in memory
/// </summary>
public class InMemoryMTGDeckRepository : IRepository<MTGCardDeck>
{
  #region Properties
  protected static List<MTGCardDeckDTO> Decks { get; } = new();
  protected ICardAPI<MTGCard> CardAPI { get; set; }
  #endregion

  public InMemoryMTGDeckRepository(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  #region IRepository implementation
  public virtual async Task<bool> Add(MTGCardDeck item)
  {
    if (!await Exists(item.Name))
    {
      Decks.Add(new MTGCardDeckDTO(item));
      return true;
    }
    else { return false; }
  }

  public virtual async Task<bool> AddOrUpdate(MTGCardDeck item)
  {
    if (await Exists(item.Name)) { return await Update(item); }
    else { return await Add(item); }
  }

  public virtual async Task<bool> Exists(string name) => await Task.Run(() => Decks.FirstOrDefault(x => x.Name == name) != null);

  public virtual async Task<IEnumerable<MTGCardDeck>> Get(Expression<Func<MTGCardDeckDTO, object>>[] Includes = null) => await Task.WhenAll(Decks.Select(x => x.AsMTGCardDeck(CardAPI)));

  public virtual async Task<MTGCardDeck> Get(string name, Expression<Func<MTGCardDeck, object>>[] Includes = null) => await Decks.FirstOrDefault(x => x.Name == name)?.AsMTGCardDeck(CardAPI);

  public virtual async Task<bool> Delete(MTGCardDeck item) => await Task.Run(() => Decks.Remove(Decks.FirstOrDefault(x => x.Name == item.Name)));

  public virtual async Task<bool> Update(MTGCardDeck item)
  {
    var index = await Task.Run(() => Decks.FindIndex(x => x.Name == item.Name));
    if (index >= 0) { Decks[index] = new MTGCardDeckDTO(item); return true; }
    else { return false; }
  }
  #endregion
}
