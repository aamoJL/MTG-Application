using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGApplication.General.Services.Databases.Migrations
{
    /// <inheritdoc />
    public partial class MTGCardGroup001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "MTGCards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                table: "MTGCards");
        }
    }
}
