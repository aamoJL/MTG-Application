using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGApplication.Database.Migrations
{
  /// <inheritdoc />
  public partial class Removelist001 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
          name: "DeckRemovelistId",
          table: "MTGCards",
          type: "INTEGER",
          nullable: true);

      migrationBuilder.CreateIndex(
          name: "IX_MTGCards_DeckRemovelistId",
          table: "MTGCards",
          column: "DeckRemovelistId");

      migrationBuilder.AddForeignKey(
          name: "FK_MTGCards_MTGDecks_DeckRemovelistId",
          table: "MTGCards",
          column: "DeckRemovelistId",
          principalTable: "MTGDecks",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_MTGCards_MTGDecks_DeckRemovelistId",
          table: "MTGCards");

      migrationBuilder.DropIndex(
          name: "IX_MTGCards_DeckRemovelistId",
          table: "MTGCards");

      migrationBuilder.DropColumn(
          name: "DeckRemovelistId",
          table: "MTGCards");
    }
  }
}
