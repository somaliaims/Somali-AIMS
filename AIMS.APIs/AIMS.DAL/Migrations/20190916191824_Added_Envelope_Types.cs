using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Envelope_Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EnvelopeYearlyBreakups",
                table: "EnvelopeYearlyBreakups");

            migrationBuilder.AddColumn<int>(
                name: "EnvelopeTypeId",
                table: "EnvelopeYearlyBreakups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnvelopeYearlyBreakups",
                table: "EnvelopeYearlyBreakups",
                columns: new[] { "EnvelopeTypeId", "YearId" });

            migrationBuilder.CreateTable(
                name: "EnvelopeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvelopeTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnvelopeYearlyBreakups_EnvelopeId",
                table: "EnvelopeYearlyBreakups",
                column: "EnvelopeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnvelopeYearlyBreakups_EnvelopeTypes_EnvelopeTypeId",
                table: "EnvelopeYearlyBreakups",
                column: "EnvelopeTypeId",
                principalTable: "EnvelopeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnvelopeYearlyBreakups_EnvelopeTypes_EnvelopeTypeId",
                table: "EnvelopeYearlyBreakups");

            migrationBuilder.DropTable(
                name: "EnvelopeTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnvelopeYearlyBreakups",
                table: "EnvelopeYearlyBreakups");

            migrationBuilder.DropIndex(
                name: "IX_EnvelopeYearlyBreakups_EnvelopeId",
                table: "EnvelopeYearlyBreakups");

            migrationBuilder.DropColumn(
                name: "EnvelopeTypeId",
                table: "EnvelopeYearlyBreakups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnvelopeYearlyBreakups",
                table: "EnvelopeYearlyBreakups",
                columns: new[] { "EnvelopeId", "YearId" });
        }
    }
}
