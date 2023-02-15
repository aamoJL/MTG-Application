using MTGApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Services
{
  public interface IDeckService<T>
  {
    public Task<bool> Exists(string name);
    public Task<bool> Add(T deck);
    public Task<bool> Update(T deck);
    public Task<bool> AddOrUpdate(T deck);
    public Task<bool> Remove(T deck);
    public Task<IEnumerable<T>> Get();
    public Task<T> Get(string name);
  }

  public class InMemoryMTGDeckService : IDeckService<MTGCardDeck>
  {
    protected static List<MTGCardDeckDTO> Decks { get; } = new();

    public async Task<bool> Exists(string name)
    {
      return await Task.Run(() => Decks.FirstOrDefault(x => x.Name == name) != null);
    }
    public async Task<bool> Add(MTGCardDeck deck)
    {
      if(!await Exists(deck.Name))
      {
        Decks.Add(new MTGCardDeckDTO(deck));
        return true;
      }
      else { return false; }
    }
    public async Task<bool> Update(MTGCardDeck deck)
    {
      var index = await Task.Run(() => Decks.FindIndex(x => x.Name == deck.Name));
      if(index >= 0) { Decks[index] = new MTGCardDeckDTO(deck); return true; }
      else { return false; }
    }
    public async Task<bool> AddOrUpdate(MTGCardDeck deck)
    {
      if(await Exists(deck.Name)) { return await Update(deck); } 
      else { return await Add(deck); }
    }
    public async Task<bool> Remove(MTGCardDeck deck)
    {
      return await Task.Run(() => Decks.Remove(Decks.FirstOrDefault(x => x.Name == deck.Name)));
    }
    public async Task<IEnumerable<MTGCardDeck>> Get()
    {
      return await Task.Run(() => Decks.Select(x => x.AsMTGCardDeck()));
    }
    public async Task<MTGCardDeck> Get(string name)
    {
      return await Task.Run(() => Decks.FirstOrDefault(x => x.Name == name)?.AsMTGCardDeck());
    }
  }
}
