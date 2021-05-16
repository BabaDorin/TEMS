using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class NotificationSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommonNotificationId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommonNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendEmail = table.Column<bool>(type: "bit", nullable: false),
                    SendSMS = table.Column<bool>(type: "bit", nullable: false),
                    SendPush = table.Column<bool>(type: "bit", nullable: false),
                    SendBrowser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendEmail = table.Column<bool>(type: "bit", nullable: false),
                    SendSMS = table.Column<bool>(type: "bit", nullable: false),
                    SendPush = table.Column<bool>(type: "bit", nullable: false),
                    SendBrowser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CommonNotificationId",
                table: "AspNetUsers",
                column: "CommonNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserID",
                table: "UserNotifications",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CommonNotifications_CommonNotificationId",
                table: "AspNetUsers",
                column: "CommonNotificationId",
                principalTable: "CommonNotifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CommonNotifications_CommonNotificationId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CommonNotifications");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CommonNotificationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CommonNotificationId",
                table: "AspNetUsers");
        }
    }
}
