using Microsoft.EntityFrameworkCore.Migrations;

namespace AIMS.DAL.Migrations
{
    public partial class Added_Sublocations_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubLocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubLocation = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EFProjectSubLocations",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    SubLocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFProjectSubLocations", x => new { x.ProjectId, x.LocationId, x.SubLocationId });
                    table.ForeignKey(
                        name: "FK_EFProjectSubLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EFProjectSubLocations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EFProjectSubLocations_SubLocations_SubLocationId",
                        column: x => x.SubLocationId,
                        principalTable: "SubLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EFProjectSubLocations_LocationId",
                table: "EFProjectSubLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EFProjectSubLocations_SubLocationId",
                table: "EFProjectSubLocations",
                column: "SubLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SubLocations_LocationId",
                table: "SubLocations",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EFProjectSubLocations");

            migrationBuilder.DropTable(
                name: "SubLocations");
        }
    }
}
