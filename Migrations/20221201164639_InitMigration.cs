using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGApplication.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListsOfCards",
                columns: table => new
                {
                    ListOfCardsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListsOfCards", x => x.ListOfCardsId);
                });

            migrationBuilder.CreateTable(
                name: "CardDecks",
                columns: table => new
                {
                    CardDeckId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CardlistId = table.Column<int>(type: "INTEGER", nullable: true),
                    WishlistId = table.Column<int>(type: "INTEGER", nullable: true),
                    MaybelistId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDecks", x => x.CardDeckId);
                    table.ForeignKey(
                        name: "FK_CardDecks_ListsOfCards_CardlistId",
                        column: x => x.CardlistId,
                        principalTable: "ListsOfCards",
                        principalColumn: "ListOfCardsId");
                    table.ForeignKey(
                        name: "FK_CardDecks_ListsOfCards_MaybelistId",
                        column: x => x.MaybelistId,
                        principalTable: "ListsOfCards",
                        principalColumn: "ListOfCardsId");
                    table.ForeignKey(
                        name: "FK_CardDecks_ListsOfCards_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "ListsOfCards",
                        principalColumn: "ListOfCardsId");
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ScryfallId = table.Column<string>(type: "varchar(36)", nullable: true),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    CardListId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardId);
                    table.ForeignKey(
                        name: "FK_Cards_ListsOfCards_CardListId",
                        column: x => x.CardListId,
                        principalTable: "ListsOfCards",
                        principalColumn: "ListOfCardsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardDecks_CardlistId",
                table: "CardDecks",
                column: "CardlistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardDecks_MaybelistId",
                table: "CardDecks",
                column: "MaybelistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardDecks_Name",
                table: "CardDecks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardDecks_WishlistId",
                table: "CardDecks",
                column: "WishlistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardListId",
                table: "Cards",
                column: "CardListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardDecks");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "ListsOfCards");
        }
    }
}
