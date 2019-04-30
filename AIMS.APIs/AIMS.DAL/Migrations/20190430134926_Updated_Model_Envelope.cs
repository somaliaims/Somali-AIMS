using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Model_Envelope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Envelope",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "ExpectedAmount",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "ManualAmount",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Envelope");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Envelope",
                table: "Envelope",
                column: "FunderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Envelope",
                table: "Envelope");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Envelope",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedAmount",
                table: "Envelope",
                type: "decimal(9, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ManualAmount",
                table: "Envelope",
                type: "decimal(9, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Envelope",
                type: "decimal(9, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Envelope",
                table: "Envelope",
                columns: new[] { "FunderId", "Year" });
        }
    }
}
