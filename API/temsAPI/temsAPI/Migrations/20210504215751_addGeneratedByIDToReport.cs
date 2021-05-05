using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class addGeneratedByIDToReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_GeneratedById",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "GeneratedById",
                table: "Reports",
                newName: "GeneratedByID");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_GeneratedById",
                table: "Reports",
                newName: "IX_Reports_GeneratedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_GeneratedByID",
                table: "Reports",
                column: "GeneratedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_GeneratedByID",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "GeneratedByID",
                table: "Reports",
                newName: "GeneratedById");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_GeneratedByID",
                table: "Reports",
                newName: "IX_Reports_GeneratedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_GeneratedById",
                table: "Reports",
                column: "GeneratedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
