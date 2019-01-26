using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Disbursement_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ProjectSectors");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "ProjectSectors");

            migrationBuilder.RenameColumn(
                name: "Percentage",
                table: "ProjectDisbursements",
                newName: "Amount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "ProjectDisbursements",
                newName: "Percentage");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "ProjectSectors",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "ProjectSectors",
                type: "decimal(9, 2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
