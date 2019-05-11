using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Manual_ExRate_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EFManualExchangeRates",
                columns: table => new
                {
                    DefaultCurrencyId = table.Column<int>(nullable: false),
                    NationalCurrencyId = table.Column<int>(nullable: false),
                    Dated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFManualExchangeRates", x => new { x.DefaultCurrencyId, x.NationalCurrencyId, x.Dated });
                    table.ForeignKey(
                        name: "FK_EFManualExchangeRates_Currencies_DefaultCurrencyId",
                        column: x => x.DefaultCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EFManualExchangeRates_Currencies_NationalCurrencyId",
                        column: x => x.NationalCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EFManualExchangeRates_NationalCurrencyId",
                table: "EFManualExchangeRates",
                column: "NationalCurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EFManualExchangeRates");
        }
    }
}
