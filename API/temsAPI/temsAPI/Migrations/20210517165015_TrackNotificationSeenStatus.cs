using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class TrackNotificationSeenStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonNotificationTEMSUser");

            migrationBuilder.AddColumn<bool>(
                name: "Seen",
                table: "UserNotifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserCommonNotification",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Seen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCommonNotification", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_UserCommonNotification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCommonNotification_CommonNotifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "CommonNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCommonNotification_NotificationId",
                table: "UserCommonNotification",
                column: "NotificationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCommonNotification");

            migrationBuilder.DropColumn(
                name: "Seen",
                table: "UserNotifications");

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
    }
}
