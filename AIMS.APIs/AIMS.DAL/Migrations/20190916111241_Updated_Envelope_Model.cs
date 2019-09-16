using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Envelope_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Envelope",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "SectorAmountsBreakup",
                table: "Envelope");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Envelope",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Envelope",
                table: "Envelope",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EnvelopeYearlyBreakups",
                columns: table => new
                {
                    EnvelopeId = table.Column<int>(nullable: false),
                    YearId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvelopeYearlyBreakups", x => new { x.EnvelopeId, x.YearId });
                    table.ForeignKey(
                        name: "FK_EnvelopeYearlyBreakups_Envelope_EnvelopeId",
                        column: x => x.EnvelopeId,
                        principalTable: "Envelope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnvelopeYearlyBreakups_FinancialYears_YearId",
                        column: x => x.YearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Envelope_FunderId",
                table: "Envelope",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvelopeYearlyBreakups_YearId",
                table: "EnvelopeYearlyBreakups",
                column: "YearId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvelopeYearlyBreakups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Envelope",
                table: "Envelope");

            migrationBuilder.DropIndex(
                name: "IX_Envelope_FunderId",
                table: "Envelope");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Envelope");

            migrationBuilder.AddColumn<string>(
                name: "SectorAmountsBreakup",
                table: "Envelope",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Envelope",
                table: "Envelope",
                column: "FunderId");
        }
    }
}
