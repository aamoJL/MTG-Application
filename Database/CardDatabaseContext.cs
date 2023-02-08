using Microsoft.EntityFrameworkCore;
using MTGApplication.Models;

// Add-migration {name} -OutputDir "Database/Migrations"

namespace MTGApplication.Database
{
  public class CardDatabaseContext : DbContext
  {
    public DbSet<MTGCard> MTGCards { get; set; }
    public DbSet<MTGCardDeck> MTGCardDecks { get; set; }

    public string DbPath = $"database.db";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      #region MTGCard
      modelBuilder.Entity<MTGCard>()
        .HasOne(e => e.MTGCardDeckDeckCards)
        .WithMany(e => e.DeckCards)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<MTGCard>()
        .HasOne(e => e.MTGCardDeckMaybelist)
        .WithMany(e => e.Maybelist)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<MTGCard>()
        .HasOne(e => e.MTGCardDeckWishlist)
        .WithMany(e => e.Wishlist)
        .OnDelete(DeleteBehavior.Cascade);
      #endregion
    }
  }
}
