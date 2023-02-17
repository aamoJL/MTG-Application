using Microsoft.EntityFrameworkCore;
using MTGApplication.Models;
using System;
using System.IO;

namespace MTGApplication.Database
{
  // Add-migration 001 -OutputDir "Database/Migrations"

  public class CardDbContext : DbContext
  {
    public DbSet<MTGCardDeckDTO> MTGDecks { get; set; }
    public DbSet<CardDTO> MTGCards { get; set; }

    public string DbFileName { get; } = "database.db";
    public string DbPath { get; }

    public CardDbContext()
    {
      var folder = Environment.SpecialFolder.LocalApplicationData;
      var path = Environment.GetFolderPath(folder);
      DbPath = Path.Join(path, DbFileName);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

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
