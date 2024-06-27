using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace MTGApplication.General.Databases
{
  /// <inheritdoc />
  public partial class _001 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "MTGDecks",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Name = table.Column<string>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_MTGDecks", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "MTGCards",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Name = table.Column<string>(type: "TEXT", nullable: true),
            ScryfallId = table.Column<Guid>(type: "TEXT", nullable: false),
            Count = table.Column<int>(type: "INTEGER", nullable: false),
            DeckCardsId = table.Column<int>(type: "INTEGER", nullable: true),
            DeckWishlistId = table.Column<int>(type: "INTEGER", nullable: true),
            DeckMaybelistId = table.Column<int>(type: "INTEGER", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_MTGCards", x => x.Id);
            table.ForeignKey(
                      name: "FK_MTGCards_MTGDecks_DeckCardsId",
                      column: x => x.DeckCardsId,
                      principalTable: "MTGDecks",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_MTGCards_MTGDecks_DeckMaybelistId",
                      column: x => x.DeckMaybelistId,
                      principalTable: "MTGDecks",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_MTGCards_MTGDecks_DeckWishlistId",
                      column: x => x.DeckWishlistId,
                      principalTable: "MTGDecks",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_MTGCards_DeckCardsId",
          table: "MTGCards",
          column: "DeckCardsId");

      migrationBuilder.CreateIndex(
          name: "IX_MTGCards_DeckMaybelistId",
          table: "MTGCards",
          column: "DeckMaybelistId");

      migrationBuilder.CreateIndex(
          name: "IX_MTGCards_DeckWishlistId",
          table: "MTGCards",
          column: "DeckWishlistId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "MTGCards");

      migrationBuilder.DropTable(
          name: "MTGDecks");
    }
  }
}
