using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.data.migrations
{
    public partial class DeleteFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Sensors_SensorId",
                table: "Records");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Sensors_SensorId",
                table: "Records",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Sensors_SensorId",
                table: "Records");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Sensors_SensorId",
                table: "Records",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id");
        }
    }
}
