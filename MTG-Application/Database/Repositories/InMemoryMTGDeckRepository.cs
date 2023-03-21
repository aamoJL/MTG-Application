using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Database.Repositories
{
  public class InMemoryMTGDeckRepository : IRepository<MTGCardDeck>
  {
    protected static List<MTGCardDeckDTO> Decks { get; } = new();

    public InMemoryMTGDeckRepository(ICardAPI<MTGCard> cardAPI)
    {
      CardAPI = cardAPI;
    }

    protected ICardAPI<MTGCard> CardAPI { get; set; }

    public virtual async Task<bool> Add(MTGCardDeck deck)
    {
      if (!await Exists(deck.Name))
      {
        Decks.Add(new MTGCardDeckDTO(deck));
        return true;
      }
      else { return false; }
    }
    public virtual async Task<bool> AddOrUpdate(MTGCardDeck deck)
    {
      if (await Exists(deck.Name)) { return await Update(deck); }
      else { return await Add(deck); }
    }
    public virtual async Task<bool> Exists(string name)
    {
      return await Task.Run(() => Decks.FirstOrDefault(x => x.Name == name) != null);
    }
    public virtual async Task<IEnumerable<MTGCardDeck>> Get()
    {
      return await Task.WhenAll(Decks.Select(x => x.AsMTGCardDeck(CardAPI)));
    }
    public virtual async Task<MTGCardDeck> Get(string name)
    {
      return await Decks.FirstOrDefault(x => x.Name == name).AsMTGCardDeck(CardAPI);
    }
    public virtual async Task<bool> Remove(MTGCardDeck deck)
    {
      return await Task.Run(() => Decks.Remove(Decks.FirstOrDefault(x => x.Name == deck.Name)));
    }
    public virtual async Task<bool> Update(MTGCardDeck deck)
    {
      var index = await Task.Run(() => Decks.FindIndex(x => x.Name == deck.Name));
      if (index >= 0) { Decks[index] = new MTGCardDeckDTO(deck); return true; }
      else { return false; }
    }
  }
}
