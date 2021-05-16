using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class UserNotificationCollections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CommonNotifications_CommonNotificationId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CommonNotificationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CommonNotificationId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "CommonNotificationTEMSUser",
                columns: table => new
                {
                    CommonNotificationsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SendToId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonNotificationTEMSUser", x => new { x.CommonNotificationsId, x.SendToId });
                    table.ForeignKey(
                        name: "FK_CommonNotificationTEMSUser_AspNetUsers_SendToId",
                        column: x => x.SendToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonNotificationTEMSUser_CommonNotifications_CommonNotificationsId",
                        column: x => x.CommonNotificationsId,
                        principalTable: "CommonNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationTEMSUser_SendToId",
                table: "CommonNotificationTEMSUser",
                column: "SendToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonNotificationTEMSUser");

            migrationBuilder.AddColumn<string>(
                name: "CommonNotificationId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CommonNotificationId",
                table: "AspNetUsers",
                column: "CommonNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CommonNotifications_CommonNotificationId",
                table: "AspNetUsers",
                column: "CommonNotificationId",
                principalTable: "CommonNotifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
