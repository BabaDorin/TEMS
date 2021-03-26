using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddingCreatedByAndDateCreatedToReportTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ReportTemplates",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ReportTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_CreatedById",
                table: "ReportTemplates",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_CreatedById",
                table: "ReportTemplates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_CreatedById",
                table: "ReportTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ReportTemplates_CreatedById",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ReportTemplates");
        }
    }
}
