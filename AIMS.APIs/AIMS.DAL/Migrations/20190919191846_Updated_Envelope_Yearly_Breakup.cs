using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Updated_Envelope_Yearly_Breakup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EnvelopeYearlyBreakups",
                table: "EnvelopeYearlyBreakups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnvelopeYearlyBreakups",
                table: "EnvelopeYearlyBreakups",
                columns: new[] { "EnvelopeTypeId", "EnvelopeId", "YearId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EnvelopeYearlyBreakups",
                table: "EnvelopeYearlyBreakups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnvelopeYearlyBreakups",
                table: "EnvelopeYearlyBreakups",
                columns: new[] { "EnvelopeTypeId", "YearId" });
        }
    }
}
