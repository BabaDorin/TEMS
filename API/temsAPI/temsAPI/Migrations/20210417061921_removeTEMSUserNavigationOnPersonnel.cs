using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class removeTEMSUserNavigationOnPersonnel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_AspNetUsers_TEMSUserId",
                table: "Personnel");

            migrationBuilder.DropIndex(
                name: "IX_Personnel_TEMSUserId",
                table: "Personnel");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PersonnelId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "TEMSUserId",
                table: "Personnel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonnelId",
                table: "AspNetUsers",
                column: "PersonnelId",
                unique: true,
                filter: "[PersonnelId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PersonnelId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "TEMSUserId",
                table: "Personnel",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_TEMSUserId",
                table: "Personnel",
                column: "TEMSUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonnelId",
                table: "AspNetUsers",
                column: "PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personnel_AspNetUsers_TEMSUserId",
                table: "Personnel",
                column: "TEMSUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
