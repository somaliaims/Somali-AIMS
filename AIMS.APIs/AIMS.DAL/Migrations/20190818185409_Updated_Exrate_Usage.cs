using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Exrate_Usage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExchangeRatesUsageSettings",
                table: "ExchangeRatesUsageSettings");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ExchangeRatesUsageSettings",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExchangeRatesUsageSettings",
                table: "ExchangeRatesUsageSettings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRatesUsageSettings_Source_UsageSection",
                table: "ExchangeRatesUsageSettings",
                columns: new[] { "Source", "UsageSection" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExchangeRatesUsageSettings",
                table: "ExchangeRatesUsageSettings");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRatesUsageSettings_Source_UsageSection",
                table: "ExchangeRatesUsageSettings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ExchangeRatesUsageSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExchangeRatesUsageSettings",
                table: "ExchangeRatesUsageSettings",
                columns: new[] { "Source", "UsageSection", "Order" });
        }
    }
}
