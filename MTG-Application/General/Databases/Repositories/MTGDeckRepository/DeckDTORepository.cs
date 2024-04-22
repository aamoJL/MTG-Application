using MTGApplication.Database;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class DeckDTORepository : IRepository<MTGCardDeckDTO>
{
  public DeckDTORepository(CardDbContextFactory dbContextFactory = null)
  {
    DbContextFactory = dbContextFactory ?? new();
  }

  public CardDbContextFactory DbContextFactory { get; }

  public Task<bool> Add(MTGCardDeckDTO item) => throw new NotImplementedException();
  public Task<bool> AddOrUpdate(MTGCardDeckDTO item) => throw new NotImplementedException();
  public Task<bool> Exists(string name)
  {
    using var db = DbContextFactory.CreateDbContext();
    return Task.FromResult(db.MTGDecks.FirstOrDefault(x => x.Name == name) != null);
  }

  public async Task<IEnumerable<MTGCardDeckDTO>> Get(Expression<Func<MTGCardDeckDTO, object>>[] Includes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var decks = db.MTGDecks.SetIncludesOrDefault(Includes);
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(decks.ToList());
  }

  // TODO: remove task?
  public async Task<MTGCardDeckDTO> Get(string name, Expression<Func<MTGCardDeckDTO, object>>[] Includes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var deck = db.MTGDecks.Where(x => x.Name == name).SetIncludesOrDefault(Includes).FirstOrDefault();
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(deck);
  }

  public Task<bool> Remove(MTGCardDeckDTO item) => throw new NotImplementedException();
  public Task<bool> Update(MTGCardDeckDTO item) => throw new NotImplementedException();
}
