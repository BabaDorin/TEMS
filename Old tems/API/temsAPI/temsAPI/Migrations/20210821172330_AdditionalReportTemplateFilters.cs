using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AdditionalReportTemplateFilters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludeChildren",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeDefect",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeFunctional",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeInUse",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeParent",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeUnused",
                table: "ReportTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeChildren",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "IncludeDefect",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "IncludeFunctional",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "IncludeInUse",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "IncludeParent",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "IncludeUnused",
                table: "ReportTemplates");
        }
    }
}
