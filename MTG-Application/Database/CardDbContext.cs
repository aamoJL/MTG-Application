using Microsoft.EntityFrameworkCore;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;

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
    #region MTGCardDeckDTO
    modelBuilder.Entity<MTGCardDeckDTO>()
      .HasMany(c => c.DeckCards)
      .WithOne()
      .HasForeignKey("DeckCardsId")
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDeckDTO>()
      .HasMany(c => c.MaybelistCards)
      .WithOne()
      .HasForeignKey("DeckMaybelistId")
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDeckDTO>()
      .HasMany(c => c.WishlistCards)
      .WithOne()
      .HasForeignKey("DeckWishlistId")
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDeckDTO>()
      .HasMany(c => c.RemovelistCards)
      .WithOne()
      .HasForeignKey("DeckRemovelistId")
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDeckDTO>()
      .HasOne(c => c.Commander)
      .WithOne()
      .HasForeignKey<MTGCardDTO>("DeckCommanderId")
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<MTGCardDeckDTO>()
      .HasOne(c => c.CommanderPartner)
      .WithOne()
      .HasForeignKey<MTGCardDTO>("DeckCommanderPartnerId")
      .OnDelete(DeleteBehavior.Cascade);
    #endregion

    #region MTGCardCollectionDTO
    modelBuilder.Entity<MTGCardCollectionDTO>()
      .HasMany(c => c.CollectionLists)
      .WithOne()
      .HasForeignKey("CollectionId")
      .OnDelete(DeleteBehavior.Cascade);
    #endregion

    #region MTGCardCollectionListDTO
    modelBuilder.Entity<MTGCardCollectionListDTO>()
      .HasMany(c => c.Cards)
      .WithOne()
      .HasForeignKey("CollectionListId")
      .OnDelete(DeleteBehavior.Cascade);
    #endregion
  }
}
