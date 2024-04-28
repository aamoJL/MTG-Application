using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Models.CardDeck;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace MTGApplication.General.Databases;

public static partial class DbSetExtensions
{
  public static IQueryable<MTGCardDeckDTO> SetIncludesOrDefault(this IQueryable<MTGCardDeckDTO> value, Expression<Func<MTGCardDeckDTO, object>>[] includes = null)
  {
    includes ??= MTGCardDeckDTO.DefaultIncludes;

    foreach (var include in includes)
    {
      value.Include(include).Load();
    }

    return value;
  }
}
