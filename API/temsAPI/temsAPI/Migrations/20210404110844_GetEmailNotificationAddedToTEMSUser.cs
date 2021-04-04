using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class GetEmailNotificationAddedToTEMSUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GetEmailNotifications",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GetEmailNotifications",
                table: "AspNetUsers");
        }
    }
}
