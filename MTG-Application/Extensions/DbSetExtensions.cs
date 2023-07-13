using Microsoft.EntityFrameworkCore;
using MTGApplication.Models.DTOs;
using System.Linq;

namespace MTGApplication.Extensions;

public static class DbSetExtensions
{
  public static IQueryable<MTGCardDeckDTO> WithDefaultIncludes(this DbSet<MTGCardDeckDTO> dbSet)
    => dbSet.Include(x => x.DeckCards)
    .Include(x => x.WishlistCards)
    .Include(x => x.MaybelistCards)
    .Include(x => x.RemovelistCards)
    .Include(x => x.Commander)
    .Include(x => x.CommanderPartner);
}
