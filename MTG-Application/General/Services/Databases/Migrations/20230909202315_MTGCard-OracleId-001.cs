using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace MTGApplication.General.Databases
{
  /// <inheritdoc />
  public partial class MTGCardOracleId001 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<Guid>(
          name: "OracleId",
          table: "MTGCards",
          type: "TEXT",
          nullable: false,
          defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "OracleId",
          table: "MTGCards");
    }
  }
}
