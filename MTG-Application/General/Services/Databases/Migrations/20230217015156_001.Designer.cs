﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MTGApplication.General.Services.Databases.Context;


#nullable disable

namespace MTGApplication.General.Databases
{
  [DbContext(typeof(CardDbContext))]
    [Migration("20230217015156_001")]
    partial class _001
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.2");

            modelBuilder.Entity("MTGApplication.Models.CardDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Count")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckCardsId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckMaybelistId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckWishlistId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ScryfallId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DeckCardsId");

                    b.HasIndex("DeckMaybelistId");

                    b.HasIndex("DeckWishlistId");

                    b.ToTable("MTGCards");
                });

            modelBuilder.Entity("MTGApplication.Models.MTGCardDeckDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MTGDecks");
                });

            modelBuilder.Entity("MTGApplication.Models.CardDTO", b =>
                {
                    b.HasOne("MTGApplication.Models.MTGCardDeckDTO", "DeckCards")
                        .WithMany("DeckCards")
                        .HasForeignKey("DeckCardsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.MTGCardDeckDTO", "DeckMaybelist")
                        .WithMany("MaybelistCards")
                        .HasForeignKey("DeckMaybelistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.MTGCardDeckDTO", "DeckWishlist")
                        .WithMany("WishlistCards")
                        .HasForeignKey("DeckWishlistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("DeckCards");

                    b.Navigation("DeckMaybelist");

                    b.Navigation("DeckWishlist");
                });

            modelBuilder.Entity("MTGApplication.Models.MTGCardDeckDTO", b =>
                {
                    b.Navigation("DeckCards");

                    b.Navigation("MaybelistCards");

                    b.Navigation("WishlistCards");
                });
#pragma warning restore 612, 618
        }
    }
}
