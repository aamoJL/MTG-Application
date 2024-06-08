using MTGApplication;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using System.Linq.Expressions;

namespace MTGApplicationTests.TestUtility.Database;

public class TestCardCollectionDTORepository : CardCollectionDTORepository, IDisposable
{
  public TestCardCollectionDTORepository(TestCardDbContextFactory ctxFactory) : base(ctxFactory)
    => AppConfig.Initialize();

  public bool AddFailure { get; set; }
  public bool UpdateFailure { get; set; }
  public bool ExistsFailure { get; set; }
  public bool GetFailure { get; set; }
  public bool DeleteFailure { get; set; }

  public void Dispose() => GC.SuppressFinalize(this);

  public override Task<bool> Add(MTGCardCollectionDTO item) => AddFailure ? Task.FromResult(false) : base.Add(item);

  public override Task<bool> AddOrUpdate(MTGCardCollectionDTO item) => base.AddOrUpdate(item);

  public override Task<bool> Exists(string name) => ExistsFailure ? Task.FromResult(false) : base.Exists(name);

  public override async Task<IEnumerable<MTGCardCollectionDTO>> Get(Expression<Func<MTGCardCollectionDTO, object>>[]? Includes = null) => GetFailure ? await Task.FromResult(new List<MTGCardCollectionDTO>()) : await base.Get(Includes);

  public override Task<MTGCardCollectionDTO?> Get(string name, Expression<Func<MTGCardCollectionDTO, object>>[]? Includes = null) => GetFailure ? Task.FromResult(default(MTGCardCollectionDTO)) : base.Get(name, Includes);

  public override Task<bool> Delete(MTGCardCollectionDTO item) => DeleteFailure ? Task.FromResult(false) : base.Delete(item);

  public override Task<bool> Update(MTGCardCollectionDTO item) => UpdateFailure ? Task.FromResult(false) : base.Update(item);
}