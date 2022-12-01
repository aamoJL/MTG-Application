using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTGApplication
{
  public static class Database
  {
    public class CardCollectionContext : DbContext
    {
      public DbSet<Card> Cards { get; set; }
      public DbSet<CardDeck> CardDecks { get; set; }
      public DbSet<ListOfCards> ListsOfCards { get; set; }

      public string DbPath = "database.db";

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
        modelBuilder.Entity<CardDeck>().HasIndex(c => c.Name).IsUnique(true);
        modelBuilder.Entity<CardDeck>().HasOne(x => x.Cardlist).WithOne();
        modelBuilder.Entity<CardDeck>().HasOne(x => x.Wishlist).WithOne();
        modelBuilder.Entity<CardDeck>().HasOne(x => x.Maybelist).WithOne();

        modelBuilder.Entity<ListOfCards>().HasMany(x => x.Cards).WithOne(x => x.CardList).OnDelete(DeleteBehavior.Cascade);
      }
    }

    public class Card
    {
      [Key]
      public int CardId { get; set; }
      [Required]
      public string Name { get; set; }
      [Column(TypeName = "varchar(36)")]
      public string ScryfallId { get; set; }
      public int Count { get; set; }

      [ForeignKey(nameof(CardList))]
      public int CardListId { get; set; }
      public ListOfCards CardList { get; set; }
    }

    public class CardDeck
    {
      [Key]
      public int CardDeckId { get; set; }
      public string Name { get; set; }

      [ForeignKey(nameof(Cardlist))]
      public int? CardlistId { get; set; }
      public ListOfCards Cardlist { get; set; }
      
      [ForeignKey(nameof(Wishlist))]
      public int? WishlistId { get; set; }
      public ListOfCards Wishlist { get; set; }
      
      [ForeignKey(nameof(Maybelist))]
      public int? MaybelistId { get; set; }
      public ListOfCards Maybelist { get; set; }
    }

    public class ListOfCards
    {
      [Key]
      public int ListOfCardsId { get; set; }
      public List<Card> Cards { get; set; } = new();
    }
  }
}
