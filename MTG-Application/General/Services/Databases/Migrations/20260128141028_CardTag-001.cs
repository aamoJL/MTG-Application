using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGApplication.General.Services.Databases.Migrations
{
  /// <inheritdoc />
  public partial class CardTag001 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
          name: "Tag",
          table: "MTGCards",
          type: "INTEGER",
          nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "Tag",
          table: "MTGCards");
    }
  }
}
