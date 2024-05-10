using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGApplication.General.Databases
{
    /// <inheritdoc />
    public partial class MTGCardSet001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CollectorNumber",
                table: "MTGCards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetCode",
                table: "MTGCards",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectorNumber",
                table: "MTGCards");

            migrationBuilder.DropColumn(
                name: "SetCode",
                table: "MTGCards");
        }
    }
}
