using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;

namespace MTGApplicationTests.TestUtility.Database;

public class SimpleTestCardCollectionRepository : IRepository<MTGCardCollectionDTO>
{
  public Func<MTGCardCollectionDTO, Task<bool>> AddResult { get => field ?? throw new NotImplementedException(); set; }
  public Func<MTGCardCollectionDTO, Task<bool>> DeleteResult { get => field ?? throw new NotImplementedException(); set; }
  public Func<string, Task<bool>> ExistsResult { get => field ?? throw new NotImplementedException(); set; }
  public Func<Task<IEnumerable<MTGCardCollectionDTO>>> GetAllResult { get => field ?? throw new NotImplementedException(); set; }
  public Func<string, Task<MTGCardCollectionDTO>> GetResult { get => field ?? throw new NotImplementedException(); set; }
  public Func<MTGCardCollectionDTO, Task<bool>> UpdateResult { get => field ?? throw new NotImplementedException(); set; }

  public async Task<bool> Add(MTGCardCollectionDTO item) => await AddResult(item);
  public async Task<bool> AddOrUpdate(MTGCardCollectionDTO item) => await AddResult(item);
  public async Task<bool> Delete(MTGCardCollectionDTO item) => await DeleteResult(item);
  public async Task<bool> Exists(string name) => await ExistsResult(name);
  public async Task<IEnumerable<MTGCardCollectionDTO>> Get(Action<DbSet<MTGCardCollectionDTO>> setIncludes = null) => await GetAllResult();
  public async Task<MTGCardCollectionDTO> Get(string name, Action<DbSet<MTGCardCollectionDTO>> setIncludes = null) => await GetResult(name);
  public async Task<bool> Update(MTGCardCollectionDTO item) => await UpdateResult(item);
}