using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Repositories;

namespace MTGApplicationTests.TestUtility.Database;

public class SimpleTestRepository<T> : IRepository<T> where T : class
{
  public Func<T, Task<bool>> AddResult { get => field ?? throw new NotImplementedException(nameof(AddResult)); set; }
  public Func<T, Task<bool>> DeleteResult { get => field ?? throw new NotImplementedException(nameof(DeleteResult)); set; }
  public Func<string, Task<bool>> ExistsResult { get => field ?? throw new NotImplementedException(nameof(ExistsResult)); set; }
  public Func<Task<IEnumerable<T>>> GetAllResult { get => field ?? throw new NotImplementedException(nameof(GetAllResult)); set; }
  public Func<string, Task<T>> GetResult { get => field ?? throw new NotImplementedException(nameof(GetResult)); set; }
  public Func<T, Task<bool>> UpdateResult { get => field ?? throw new NotImplementedException(nameof(UpdateResult)); set; }

  public async Task<bool> Add(T item) => await AddResult(item);
  public async Task<bool> AddOrUpdate(T item) => await AddResult(item);
  public async Task<bool> Delete(T item) => await DeleteResult(item);
  public async Task<bool> Exists(string name) => await ExistsResult(name);
  public async Task<IEnumerable<T>> Get(Action<DbSet<T>> setIncludes = null) => await GetAllResult();
  public async Task<T> Get(string name, Action<DbSet<T>> setIncludes = null) => await GetResult(name);
  public async Task<bool> Update(T item) => await UpdateResult(item);
}