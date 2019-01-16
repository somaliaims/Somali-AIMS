using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Location_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Locations",
                type: "decimal(9, 5)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Locations",
                type: "decimal(9, 5)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 5)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Locations",
                type: "decimal(9, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 5)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Locations",
                type: "decimal(9, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 5)",
                oldNullable: true);
        }
    }
}
