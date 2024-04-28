using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.Models.DTOs;

// Add-migration {name}-001 -OutputDir "Database/Migrations"

namespace MTGApplication.Database;

/// <summary>
/// Card database context for Entity Framework Core
/// </summary>
public class CardDbContext : DbContext
{
  public CardDbContext(DbContextOptions options) : base(options) { }

  #region Properties
  public DbSet<MTGCardDeckDTO> MTGDecks { get; set; }
  public DbSet<MTGCardDTO> MTGCards { get; set; }
  public DbSet<MTGCardCollectionDTO> MTGCardCollections { get; set; }
  public DbSet<MTGCardCollectionListDTO> MTGCardCollectionLists { get; set; }
  #endregion

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    OnMTGCardDeckDTOCreating(modelBuilder);
    OnMTGCardCollectionDTOCreating(modelBuilder);
    OnMTGCardCollectionListDTOCreating(modelBuilder);
  }

  /// <summary>
  /// <see cref="MTGCardDeckDTO"/> model creation
  /// </summary>
  protected void OnMTGCardDeckDTOCreating(ModelBuilder modelBuilder)
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
  protected void OnMTGCardCollectionDTOCreating(ModelBuilder modelBuilder)
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
  protected void OnMTGCardCollectionListDTOCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<MTGCardCollectionListDTO>()
      .HasMany(c => c.Cards)
      .WithOne()
      .HasForeignKey("CollectionListId")
      .OnDelete(DeleteBehavior.Cascade);
  }
}
