using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGApplication.Database.Migrations
{
  /// <inheritdoc />
  public partial class MTGDeckCommander001 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
          name: "DeckCommanderId",
          table: "MTGCards",
          type: "INTEGER",
          nullable: true);

      migrationBuilder.AddColumn<int>(
          name: "DeckCommanderPartnerId",
          table: "MTGCards",
          type: "INTEGER",
          nullable: true);

      migrationBuilder.CreateIndex(
          name: "IX_MTGCards_DeckCommanderId",
          table: "MTGCards",
          column: "DeckCommanderId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_MTGCards_DeckCommanderPartnerId",
          table: "MTGCards",
          column: "DeckCommanderPartnerId",
          unique: true);

      migrationBuilder.AddForeignKey(
          name: "FK_MTGCards_MTGDecks_DeckCommanderId",
          table: "MTGCards",
          column: "DeckCommanderId",
          principalTable: "MTGDecks",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_MTGCards_MTGDecks_DeckCommanderPartnerId",
          table: "MTGCards",
          column: "DeckCommanderPartnerId",
          principalTable: "MTGDecks",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_MTGCards_MTGDecks_DeckCommanderId",
          table: "MTGCards");

      migrationBuilder.DropForeignKey(
          name: "FK_MTGCards_MTGDecks_DeckCommanderPartnerId",
          table: "MTGCards");

      migrationBuilder.DropIndex(
          name: "IX_MTGCards_DeckCommanderId",
          table: "MTGCards");

      migrationBuilder.DropIndex(
          name: "IX_MTGCards_DeckCommanderPartnerId",
          table: "MTGCards");

      migrationBuilder.DropColumn(
          name: "DeckCommanderId",
          table: "MTGCards");

      migrationBuilder.DropColumn(
          name: "DeckCommanderPartnerId",
          table: "MTGCards");
    }
  }
}
