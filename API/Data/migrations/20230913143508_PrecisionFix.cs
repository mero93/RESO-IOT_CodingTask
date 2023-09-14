using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.data.migrations
{
    public partial class PrecisionFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Sensors",
                type: "decimal(12,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Sensors",
                type: "decimal(12,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Records",
                type: "decimal(12,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Records",
                type: "decimal(12,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Sensors",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Sensors",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Records",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Records",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,3)");
        }
    }
}
