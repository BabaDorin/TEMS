using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class RoomLabelsEntityAdded_ManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomLabels_LabelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_LabelId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LabelId",
                table: "Rooms");

            migrationBuilder.CreateTable(
                name: "RoomRoomLabel",
                columns: table => new
                {
                    LabelsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRoomLabel", x => new { x.LabelsId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_RoomRoomLabel_RoomLabels_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "RoomLabels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomRoomLabel_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomRoomLabel_RoomsId",
                table: "RoomRoomLabel",
                column: "RoomsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomRoomLabel");

            migrationBuilder.AddColumn<string>(
                name: "LabelId",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_LabelId",
                table: "Rooms",
                column: "LabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomLabels_LabelId",
                table: "Rooms",
                column: "LabelId",
                principalTable: "RoomLabels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
