using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddTable_UserWithBlacklistedTokens_ForeignKeyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_UserWithBlacklistedTokens_AspNetUsers_UserID",
                table: "UserWithBlacklistedTokens",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWithBlacklistedTokens_AspNetUsers_UserID",
                table: "UserWithBlacklistedTokens");
        }
    }
}
