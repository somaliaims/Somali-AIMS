using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Currency_MRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                table: "ManualExchangeRates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalCurrency",
                table: "ManualExchangeRates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                table: "ManualExchangeRates");

            migrationBuilder.DropColumn(
                name: "NationalCurrency",
                table: "ManualExchangeRates");
        }
    }
}
