using Microsoft.EntityFrameworkCore;
using MTGApplication.Models;

// Add-migration 001 -OutputDir "Database/Migrations"

namespace MTGApplication.Database;

/// <summary>
/// Card database context for Entity Framework Core
/// </summary>
public class CardDbContext : DbContext
{
  public DbSet<MTGCardDeckDTO> MTGDecks { get; set; }
  public DbSet<MTGCardDTO> MTGCards { get; set; }
  public DbSet<MTGCardCollectionDTO> MTGCardCollections { get; set; }
  public DbSet<MTGCardCollectionListDTO> MTGCardCollectionLists { get; set; }

  public CardDbContext(DbContextOptions options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<MTGCardDTO>()
      .HasOne(e => e.DeckCards)
      .WithMany(e => e.DeckCards)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDTO>()
      .HasOne(e => e.DeckMaybelist)
      .WithMany(e => e.MaybelistCards)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDTO>()
      .HasOne(e => e.DeckWishlist)
      .WithMany(e => e.WishlistCards)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDTO>()
      .HasOne(e => e.DeckRemovelist)
      .WithMany(e => e.RemovelistCards)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDTO>()
      .HasOne(e => e.CollectionList)
      .WithMany(e => e.Cards)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDeckDTO>()
      .HasOne(e => e.Commander)
      .WithOne()
      .HasForeignKey<MTGCardDTO>(e => e.DeckCommanderId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDeckDTO>()
      .HasOne(e => e.CommanderPartner)
      .WithOne()
      .HasForeignKey<MTGCardDTO>(e => e.DeckCommanderPartnerId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardCollectionListDTO>()
      .HasOne(e => e.Collection)
      .WithMany(e => e.CollectionLists)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
