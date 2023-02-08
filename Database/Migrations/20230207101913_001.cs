using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGApplication.Database.Migrations
{
    /// <inheritdoc />
    public partial class _001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MTGCardDecks",
                columns: table => new
                {
                    MTGCardDeckId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTGCardDecks", x => x.MTGCardDeckId);
                });

            migrationBuilder.CreateTable(
                name: "MTGCards",
                columns: table => new
                {
                    MTGCardId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ScryfallId = table.Column<string>(type: "varchar(36)", nullable: true),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    MTGCardDeckDeckCardsId = table.Column<int>(type: "INTEGER", nullable: true),
                    MTGCardDeckMaybelistId = table.Column<int>(type: "INTEGER", nullable: true),
                    MTGCardDeckWishlistId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTGCards", x => x.MTGCardId);
                    table.ForeignKey(
                        name: "FK_MTGCards_MTGCardDecks_MTGCardDeckDeckCardsId",
                        column: x => x.MTGCardDeckDeckCardsId,
                        principalTable: "MTGCardDecks",
                        principalColumn: "MTGCardDeckId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MTGCards_MTGCardDecks_MTGCardDeckMaybelistId",
                        column: x => x.MTGCardDeckMaybelistId,
                        principalTable: "MTGCardDecks",
                        principalColumn: "MTGCardDeckId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MTGCards_MTGCardDecks_MTGCardDeckWishlistId",
                        column: x => x.MTGCardDeckWishlistId,
                        principalTable: "MTGCardDecks",
                        principalColumn: "MTGCardDeckId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MTGCards_MTGCardDeckDeckCardsId",
                table: "MTGCards",
                column: "MTGCardDeckDeckCardsId");

            migrationBuilder.CreateIndex(
                name: "IX_MTGCards_MTGCardDeckMaybelistId",
                table: "MTGCards",
                column: "MTGCardDeckMaybelistId");

            migrationBuilder.CreateIndex(
                name: "IX_MTGCards_MTGCardDeckWishlistId",
                table: "MTGCards",
                column: "MTGCardDeckWishlistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MTGCards");

            migrationBuilder.DropTable(
                name: "MTGCardDecks");
        }
    }
}
