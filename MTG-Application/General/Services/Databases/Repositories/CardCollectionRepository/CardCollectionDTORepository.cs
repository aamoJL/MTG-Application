using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Databases;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.CardCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
public class CardCollectionDTORepository(CardDbContextFactory dbContextFactory = null) : IRepository<MTGCardCollectionDTO>
{
  public CardDbContextFactory DbContextFactory { get; } = dbContextFactory ?? new();

  public virtual async Task<bool> Add(MTGCardCollectionDTO item)
  {
    throw new NotImplementedException();
  }

  public virtual async Task<bool> AddOrUpdate(MTGCardCollectionDTO item)
  {
    throw new NotImplementedException();
  }

  public virtual async Task<bool> Delete(MTGCardCollectionDTO item)
  {
    throw new NotImplementedException();
  }

  public virtual async Task<bool> Exists(string name)
  {
    throw new NotImplementedException();
  }

  public virtual async Task<IEnumerable<MTGCardCollectionDTO>> Get(Expression<Func<MTGCardCollectionDTO, object>>[] includes = null)
    => await Get((items) => items.SetDefaultIncludesOrEmpty(includes.Length != 0));

  // TODO: test if this works and replace the expression version with this
  public async Task<IEnumerable<MTGCardCollectionDTO>> Get(Action<DbSet<MTGCardCollectionDTO>> setIncludes)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;

    setIncludes?.Invoke(db.MTGCardCollections);

    var items = db.MTGCardCollections;
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(items.ToList());
  }

  public virtual async Task<MTGCardCollectionDTO> Get(string name, Expression<Func<MTGCardCollectionDTO, object>>[] includes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var item = db.MTGCardCollections.Where(x => x.Name == name).SetDefaultIncludesOrEmpty(includes?.Length != 0).FirstOrDefault();
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(item);
  }

  public virtual async Task<bool> Update(MTGCardCollectionDTO item)
  {
    throw new NotImplementedException();
  }

  public static void SetDefaultIncludes(DbSet<MTGCardCollectionDTO> items)
    => items.Include(x => x.CollectionLists).ThenInclude(x => x.Cards);
}
