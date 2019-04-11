using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Update_ExRates_Settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "APIKeyOpenExchangeRates",
                table: "ExchangeRatesSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                table: "ExchangeRates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APIKeyOpenExchangeRates",
                table: "ExchangeRatesSettings");

            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                table: "ExchangeRates");
        }
    }
}
