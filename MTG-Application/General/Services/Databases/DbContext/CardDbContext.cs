using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;

// Add-migration {name}-001 -OutputDir "General/Services/Databases/Migrations"

namespace MTGApplication.General.Services.Databases;

/// <summary>
/// Card database context for Entity Framework Core
/// </summary>
public class CardDbContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<MTGCardDeckDTO> MTGDecks { get; set; }
  public DbSet<MTGCardDTO> MTGCards { get; set; }
  public DbSet<MTGCardCollectionDTO> MTGCardCollections { get; set; }
  public DbSet<MTGCardCollectionListDTO> MTGCardCollectionLists { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    OnMTGCardDeckDTOCreating(modelBuilder);
    OnMTGCardCollectionDTOCreating(modelBuilder);
    OnMTGCardCollectionListDTOCreating(modelBuilder);
  }

  /// <summary>
  /// <see cref="MTGCardDeckDTO"/> model creation
  /// </summary>
  protected static void OnMTGCardDeckDTOCreating(ModelBuilder modelBuilder)
  {
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
  }

  /// <summary>
  /// <see cref="MTGCardCollectionDTO"/> model creation
  /// </summary>
  protected static void OnMTGCardCollectionDTOCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<MTGCardCollectionDTO>()
      .HasMany(c => c.CollectionLists)
      .WithOne()
      .HasForeignKey("CollectionId")
      .OnDelete(DeleteBehavior.Cascade);
  }

  /// <summary>
  /// <see cref="MTGCardCollectionListDTO"/> model creation
  /// </summary>
  protected static void OnMTGCardCollectionListDTOCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<MTGCardCollectionListDTO>()
      .HasMany(c => c.Cards)
      .WithOne()
      .HasForeignKey("CollectionListId")
      .OnDelete(DeleteBehavior.Cascade);
  }
}
