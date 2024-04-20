using Microsoft.EntityFrameworkCore;
using MTGApplication.Models.DTOs;
using System.Linq;

namespace MTGApplication.Database.Extensions;

public static class DbSetExtensions
{
  public static IQueryable<MTGCardDeckDTO> WithDefaultIncludes(this IQueryable<MTGCardDeckDTO> dbSet)
  {
    dbSet.Include(x => x.DeckCards).Load();
    dbSet.Include(x => x.WishlistCards).Load();
    dbSet.Include(x => x.MaybelistCards).Load();
    dbSet.Include(x => x.RemovelistCards).Load();
    dbSet.Include(x => x.Commander).Load();
    dbSet.Include(x => x.CommanderPartner).Load();

    return dbSet;
  }
}
