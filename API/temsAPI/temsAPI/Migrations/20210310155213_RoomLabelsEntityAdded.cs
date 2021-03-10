using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class RoomLabelsEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LabelId",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomLabels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomLabels", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomLabels_LabelId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "RoomLabels");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_LabelId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LabelId",
                table: "Rooms");
        }
    }
}
