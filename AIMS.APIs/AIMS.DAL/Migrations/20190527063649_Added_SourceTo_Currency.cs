using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_SourceTo_Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManualExchangeRateSource",
                table: "ExchangeRatesSettings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Currencies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManualExchangeRateSource",
                table: "ExchangeRatesSettings");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Currencies");
        }
    }
}
