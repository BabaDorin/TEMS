using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class personnelUserAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Personnel",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonnelId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_UserId",
                table: "Personnel",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonnelId",
                table: "AspNetUsers",
                column: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Personnel_PersonnelId",
                table: "AspNetUsers",
                column: "PersonnelId",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personnel_AspNetUsers_UserId",
                table: "Personnel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Personnel_PersonnelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_AspNetUsers_UserId",
                table: "Personnel");

            migrationBuilder.DropIndex(
                name: "IX_Personnel_UserId",
                table: "Personnel");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PersonnelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Personnel");

            migrationBuilder.DropColumn(
                name: "PersonnelId",
                table: "AspNetUsers");
        }
    }
}
