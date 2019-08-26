using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Manaul_Exrate_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dated",
                table: "ManualExchangeRates");

            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                table: "ManualExchangeRates");

            migrationBuilder.DropColumn(
                name: "IsAutomatic",
                table: "ExchangeRatesSettings");

            migrationBuilder.RenameColumn(
                name: "NationalCurrency",
                table: "ManualExchangeRates",
                newName: "Currency");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ManualExchangeRates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FinancialYearEndingMonth",
                table: "ExchangeRatesSettings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FinancialYearStartingMonth",
                table: "ExchangeRatesSettings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "ManualExchangeRates");

            migrationBuilder.DropColumn(
                name: "FinancialYearEndingMonth",
                table: "ExchangeRatesSettings");

            migrationBuilder.DropColumn(
                name: "FinancialYearStartingMonth",
                table: "ExchangeRatesSettings");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "ManualExchangeRates",
                newName: "NationalCurrency");

            migrationBuilder.AddColumn<DateTime>(
                name: "Dated",
                table: "ManualExchangeRates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                table: "ManualExchangeRates",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutomatic",
                table: "ExchangeRatesSettings",
                nullable: false,
                defaultValue: false);
        }
    }
}
