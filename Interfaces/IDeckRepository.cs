using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Interfaces
{
  public interface IDeckRepository<T>
  {
    public Task<bool> Exists(string name);
    public Task<bool> Add(T deck);
    public Task<bool> Update(T deck);
    public Task<bool> AddOrUpdate(T deck);
    public Task<bool> Remove(T deck);
    public Task<IEnumerable<T>> Get();
    public Task<T> Get(string name);
  }
}
