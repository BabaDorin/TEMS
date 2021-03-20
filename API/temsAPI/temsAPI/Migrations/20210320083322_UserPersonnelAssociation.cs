using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class UserPersonnelAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_AspNetUsers_UserId",
                table: "Personnel");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Personnel",
                newName: "TEMSUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Personnel_UserId",
                table: "Personnel",
                newName: "IX_Personnel_TEMSUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personnel_AspNetUsers_TEMSUserId",
                table: "Personnel",
                column: "TEMSUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_AspNetUsers_TEMSUserId",
                table: "Personnel");

            migrationBuilder.RenameColumn(
                name: "TEMSUserId",
                table: "Personnel",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Personnel_TEMSUserId",
                table: "Personnel",
                newName: "IX_Personnel_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personnel_AspNetUsers_UserId",
                table: "Personnel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
