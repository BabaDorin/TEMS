using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class KeyRoomAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                table: "Keys",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Keys_RoomId",
                table: "Keys",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Keys_Rooms_RoomId",
                table: "Keys",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Keys_Rooms_RoomId",
                table: "Keys");

            migrationBuilder.DropIndex(
                name: "IX_Keys_RoomId",
                table: "Keys");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Keys");
        }
    }
}
