using Microsoft.EntityFrameworkCore;
using MTGApplication.Models;

namespace MTGApplication.Database
{
  // Add-migration 001 -OutputDir "Database/Migrations"

  public class CardDbContext : DbContext
  {
    public DbSet<MTGCardDeckDTO> MTGDecks { get; set; }
    public DbSet<CardDTO> MTGCards { get; set; }

    public CardDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<CardDTO>()
        .HasOne(e => e.DeckCards)
        .WithMany(e => e.DeckCards)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<CardDTO>()
        .HasOne(e => e.DeckMaybelist)
        .WithMany(e => e.MaybelistCards)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<CardDTO>()
        .HasOne(e => e.DeckWishlist)
        .WithMany(e => e.WishlistCards)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
