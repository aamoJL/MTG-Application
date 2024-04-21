using Microsoft.EntityFrameworkCore;
using MTGApplication.Models.DTOs;
using System;
using System.Linq;
using System.Linq.Expressions;

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

  public static IQueryable<MTGCardDeckDTO> SetIncludesOrDefault(this IQueryable<MTGCardDeckDTO> value, Expression<Func<MTGCardDeckDTO, object>>[] includes = null)
  {
    if (includes == null) { value.WithDefaultIncludes(); }
    else
    {
      foreach (var include in includes)
      {
        value.Include(include).Load();
      }
    }
    return value;
  }
}
