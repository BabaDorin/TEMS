using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class addingPersonnelPositionsEntity_ManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelPositions_Personnel_PersonnelId",
                table: "PersonnelPositions");

            migrationBuilder.DropIndex(
                name: "IX_PersonnelPositions_PersonnelId",
                table: "PersonnelPositions");

            migrationBuilder.DropColumn(
                name: "PersonnelId",
                table: "PersonnelPositions");

            migrationBuilder.CreateTable(
                name: "PersonnelPersonnelPosition",
                columns: table => new
                {
                    PersonnelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PositionsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelPersonnelPosition", x => new { x.PersonnelId, x.PositionsId });
                    table.ForeignKey(
                        name: "FK_PersonnelPersonnelPosition_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelPersonnelPosition_PersonnelPositions_PositionsId",
                        column: x => x.PositionsId,
                        principalTable: "PersonnelPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPersonnelPosition_PositionsId",
                table: "PersonnelPersonnelPosition",
                column: "PositionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelPersonnelPosition");

            migrationBuilder.AddColumn<string>(
                name: "PersonnelId",
                table: "PersonnelPositions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPositions_PersonnelId",
                table: "PersonnelPositions",
                column: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelPositions_Personnel_PersonnelId",
                table: "PersonnelPositions",
                column: "PersonnelId",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
