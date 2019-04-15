using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Envelope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Envelope",
                columns: table => new
                {
                    FunderId = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envelope", x => new { x.FunderId, x.Year });
                    table.ForeignKey(
                        name: "FK_Envelope_Organizations_FunderId",
                        column: x => x.FunderId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Envelope");
        }
    }
}
