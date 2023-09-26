﻿// <auto-generated />
using System;
using MTGApplication.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MTGApplication.Database.Migrations
{
    [DbContext(typeof(CardDbContext))]
    [Migration("20230909202315_MTGCard-OracleId-001")]
    partial class MTGCardOracleId001
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardCollectionDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MTGCardCollections");
                });

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardCollectionListDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CollectionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("SearchQuery")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.ToTable("MTGCardCollectionLists");
                });

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CollectionListId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Count")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckCardsId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckCommanderId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckCommanderPartnerId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckMaybelistId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckRemovelistId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeckWishlistId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OracleId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ScryfallId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CollectionListId");

                    b.HasIndex("DeckCardsId");

                    b.HasIndex("DeckCommanderId")
                        .IsUnique();

                    b.HasIndex("DeckCommanderPartnerId")
                        .IsUnique();

                    b.HasIndex("DeckMaybelistId");

                    b.HasIndex("DeckRemovelistId");

                    b.HasIndex("DeckWishlistId");

                    b.ToTable("MTGCards");
                });

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardDeckDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MTGDecks");
                });

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardCollectionListDTO", b =>
                {
                    b.HasOne("MTGApplication.Models.DTOs.MTGCardCollectionDTO", null)
                        .WithMany("CollectionLists")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardDTO", b =>
                {
                    b.HasOne("MTGApplication.Models.DTOs.MTGCardCollectionListDTO", null)
                        .WithMany("Cards")
                        .HasForeignKey("CollectionListId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.DTOs.MTGCardDeckDTO", null)
                        .WithMany("DeckCards")
                        .HasForeignKey("DeckCardsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.DTOs.MTGCardDeckDTO", null)
                        .WithOne("Commander")
                        .HasForeignKey("MTGApplication.Models.DTOs.MTGCardDTO", "DeckCommanderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.DTOs.MTGCardDeckDTO", null)
                        .WithOne("CommanderPartner")
                        .HasForeignKey("MTGApplication.Models.DTOs.MTGCardDTO", "DeckCommanderPartnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.DTOs.MTGCardDeckDTO", null)
                        .WithMany("MaybelistCards")
                        .HasForeignKey("DeckMaybelistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.DTOs.MTGCardDeckDTO", null)
                        .WithMany("RemovelistCards")
                        .HasForeignKey("DeckRemovelistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.DTOs.MTGCardDeckDTO", null)
                        .WithMany("WishlistCards")
                        .HasForeignKey("DeckWishlistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardCollectionDTO", b =>
                {
                    b.Navigation("CollectionLists");
                });

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardCollectionListDTO", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("MTGApplication.Models.DTOs.MTGCardDeckDTO", b =>
                {
                    b.Navigation("Commander");

                    b.Navigation("CommanderPartner");

                    b.Navigation("DeckCards");

                    b.Navigation("MaybelistCards");

                    b.Navigation("RemovelistCards");

                    b.Navigation("WishlistCards");
                });
#pragma warning restore 612, 618
        }
    }
}