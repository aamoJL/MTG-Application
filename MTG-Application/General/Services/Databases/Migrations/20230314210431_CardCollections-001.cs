using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGApplication.General.Databases
{
  /// <inheritdoc />
  public partial class CardCollections001 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
          name: "CollectionListId",
          table: "MTGCards",
          type: "INTEGER",
          nullable: true);

      migrationBuilder.CreateTable(
          name: "MTGCardCollections",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Name = table.Column<string>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_MTGCardCollections", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "MTGCardCollectionLists",
          columns: table => new
          {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                  .Annotation("Sqlite:Autoincrement", true),
            Name = table.Column<string>(type: "TEXT", nullable: true),
            SearchQuery = table.Column<string>(type: "TEXT", nullable: true),
            CollectionId = table.Column<int>(type: "INTEGER", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_MTGCardCollectionLists", x => x.Id);
            table.ForeignKey(
                      name: "FK_MTGCardCollectionLists_MTGCardCollections_CollectionId",
                      column: x => x.CollectionId,
                      principalTable: "MTGCardCollections",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_MTGCards_CollectionListId",
          table: "MTGCards",
          column: "CollectionListId");

      migrationBuilder.CreateIndex(
          name: "IX_MTGCardCollectionLists_CollectionId",
          table: "MTGCardCollectionLists",
          column: "CollectionId");

      migrationBuilder.AddForeignKey(
          name: "FK_MTGCards_MTGCardCollectionLists_CollectionListId",
          table: "MTGCards",
          column: "CollectionListId",
          principalTable: "MTGCardCollectionLists",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_MTGCards_MTGCardCollectionLists_CollectionListId",
          table: "MTGCards");

      migrationBuilder.DropTable(
          name: "MTGCardCollectionLists");

      migrationBuilder.DropTable(
          name: "MTGCardCollections");

      migrationBuilder.DropIndex(
          name: "IX_MTGCards_CollectionListId",
          table: "MTGCards");

      migrationBuilder.DropColumn(
          name: "CollectionListId",
          table: "MTGCards");
    }
  }
}
