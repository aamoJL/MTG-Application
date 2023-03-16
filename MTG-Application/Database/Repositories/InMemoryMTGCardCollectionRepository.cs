using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Database.Repositories
{
  public class InMemoryMTGCardCollectionRepository : IRepository<MTGCardCollection>
  {
    protected static List<MTGCardCollectionDTO> Collections { get; } = new();

    public InMemoryMTGCardCollectionRepository(ICardAPI<MTGCard> cardAPI)
    {
      CardAPI = cardAPI;
    }

    protected ICardAPI<MTGCard> CardAPI { get; set; }

    public virtual async Task<bool> Add(MTGCardCollection collection)
    {
      if (!await Exists(collection.Name))
      {
        Collections.Add(collection.AsDTO());
        return true;
      }
      else { return false; }
    }
    public virtual async Task<bool> AddOrUpdate(MTGCardCollection collection)
    {
      if (await Exists(collection.Name)) { return await Update(collection); }
      else { return await Add(collection); }
    }
    public virtual async Task<bool> Exists(string name)
    {
      return await Task.Run(() => Collections.FirstOrDefault(x => x.Name == name) != null);
    }
    public virtual async Task<IEnumerable<MTGCardCollection>> Get()
    {
      return await Task.WhenAll(Collections.Select(async x => await x.AsMTGCardCollection(CardAPI)));
    }
    public virtual async Task<MTGCardCollection> Get(string name)
    {
      return await Collections.FirstOrDefault(x => x.Name == name).AsMTGCardCollection(CardAPI);
    }
    public virtual async Task<bool> Remove(MTGCardCollection collection)
    {
      return await Task.Run(() => Collections.Remove(Collections.FirstOrDefault(x => x.Name == collection.Name)));
    }
    public virtual async Task<bool> Update(MTGCardCollection collection)
    {
      var index = await Task.Run(() => Collections.FindIndex(x => x.Name == collection.Name));
      if (index >= 0) { Collections[index] = collection.AsDTO(); return true; }
      else { return false; }
    }
  }
}
