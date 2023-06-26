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
    [Migration("20230625233725_MTG-Deck-Commander-001")]
    partial class MTGDeckCommander001
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("MTGApplication.Models.MTGCardCollectionDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MTGCardCollections");
                });

            modelBuilder.Entity("MTGApplication.Models.MTGCardCollectionListDTO", b =>
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

            modelBuilder.Entity("MTGApplication.Models.MTGCardDTO", b =>
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

                    b.Property<int?>("DeckWishlistId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
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

            modelBuilder.Entity("MTGApplication.Models.MTGCardCollectionListDTO", b =>
                {
                    b.HasOne("MTGApplication.Models.MTGCardCollectionDTO", "Collection")
                        .WithMany("CollectionLists")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("MTGApplication.Models.MTGCardDTO", b =>
                {
                    b.HasOne("MTGApplication.Models.MTGCardCollectionListDTO", "CollectionList")
                        .WithMany("Cards")
                        .HasForeignKey("CollectionListId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.MTGCardDeckDTO", "DeckCards")
                        .WithMany("DeckCards")
                        .HasForeignKey("DeckCardsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.MTGCardDeckDTO", null)
                        .WithOne("Commander")
                        .HasForeignKey("MTGApplication.Models.MTGCardDTO", "DeckCommanderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.MTGCardDeckDTO", null)
                        .WithOne("CommanderPartner")
                        .HasForeignKey("MTGApplication.Models.MTGCardDTO", "DeckCommanderPartnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.MTGCardDeckDTO", "DeckMaybelist")
                        .WithMany("MaybelistCards")
                        .HasForeignKey("DeckMaybelistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MTGApplication.Models.MTGCardDeckDTO", "DeckWishlist")
                        .WithMany("WishlistCards")
                        .HasForeignKey("DeckWishlistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("CollectionList");

                    b.Navigation("DeckCards");

                    b.Navigation("DeckMaybelist");

                    b.Navigation("DeckWishlist");
                });

            modelBuilder.Entity("MTGApplication.Models.MTGCardCollectionDTO", b =>
                {
                    b.Navigation("CollectionLists");
                });

            modelBuilder.Entity("MTGApplication.Models.MTGCardCollectionListDTO", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("MTGApplication.Models.MTGCardDeckDTO", b =>
                {
                    b.Navigation("Commander");

                    b.Navigation("CommanderPartner");

                    b.Navigation("DeckCards");

                    b.Navigation("MaybelistCards");

                    b.Navigation("WishlistCards");
                });
#pragma warning restore 612, 618
        }
    }
}
