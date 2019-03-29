using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Models : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SectorMappings",
                table: "SectorMappings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SectorMappings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectorMappings",
                table: "SectorMappings",
                columns: new[] { "SectorId", "MappedSectorId" });

            migrationBuilder.CreateTable(
                name: "ExchangeRatesSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsAutomatic = table.Column<bool>(nullable: false),
                    ManualExchangeRates = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRatesSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SectorMappings_MappedSectorId",
                table: "SectorMappings",
                column: "MappedSectorId");

            migrationBuilder.CreateIndex(
                name: "IX_SectorMappings_SectorTypeId",
                table: "SectorMappings",
                column: "SectorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectorMappings_Sectors_MappedSectorId",
                table: "SectorMappings",
                column: "MappedSectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SectorMappings_Sectors_SectorId",
                table: "SectorMappings",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SectorMappings_Sectors_SectorTypeId",
                table: "SectorMappings",
                column: "SectorTypeId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectorMappings_Sectors_MappedSectorId",
                table: "SectorMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_SectorMappings_Sectors_SectorId",
                table: "SectorMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_SectorMappings_Sectors_SectorTypeId",
                table: "SectorMappings");

            migrationBuilder.DropTable(
                name: "ExchangeRatesSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SectorMappings",
                table: "SectorMappings");

            migrationBuilder.DropIndex(
                name: "IX_SectorMappings_MappedSectorId",
                table: "SectorMappings");

            migrationBuilder.DropIndex(
                name: "IX_SectorMappings_SectorTypeId",
                table: "SectorMappings");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SectorMappings",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectorMappings",
                table: "SectorMappings",
                column: "Id");
        }
    }
}
