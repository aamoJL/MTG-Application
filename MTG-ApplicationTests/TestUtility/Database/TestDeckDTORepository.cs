using Microsoft.EntityFrameworkCore;
using MTGApplication;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;

namespace MTGApplicationTests.TestUtility.Database;
public class TestDeckDTORepository : DeckDTORepository, IDisposable
{
  public TestDeckDTORepository(TestCardDbContextFactory ctxFactory) : base(ctxFactory)
    => AppConfig.Initialize();

  public bool AddFailure { get; set; }
  public bool UpdateFailure { get; set; }
  public bool ExistsFailure { get; set; }
  public bool GetFailure { get; set; }
  public bool DeleteFailure { get; set; }

  public void Dispose() => GC.SuppressFinalize(this);

  public override Task<bool> Add(MTGCardDeckDTO item) => AddFailure ? Task.FromResult(false) : base.Add(item);

  public override Task<bool> AddOrUpdate(MTGCardDeckDTO item) => base.AddOrUpdate(item);

  public override Task<bool> Exists(string name) => ExistsFailure ? Task.FromResult(false) : base.Exists(name);

  public override async Task<IEnumerable<MTGCardDeckDTO>> Get(Action<DbSet<MTGCardDeckDTO>>? setIncludes = null) => GetFailure ? await Task.FromResult(new List<MTGCardDeckDTO>()) : await base.Get(setIncludes);

  public override Task<MTGCardDeckDTO?> Get(string name, Action<DbSet<MTGCardDeckDTO>>? setIncludes = null) => GetFailure ? Task.FromResult(default(MTGCardDeckDTO)) : base.Get(name, setIncludes);

  public override Task<bool> Delete(MTGCardDeckDTO item) => DeleteFailure ? Task.FromResult(false) : base.Delete(item);

  public override Task<bool> Update(MTGCardDeckDTO item) => UpdateFailure ? Task.FromResult(false) : base.Update(item);
}