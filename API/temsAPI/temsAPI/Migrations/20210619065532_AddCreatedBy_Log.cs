using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddCreatedBy_Log : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByID",
                table: "Logs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CreatedByID",
                table: "Logs",
                column: "CreatedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_CreatedByID",
                table: "Logs",
                column: "CreatedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_CreatedByID",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_CreatedByID",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "Logs");
        }
    }
}
