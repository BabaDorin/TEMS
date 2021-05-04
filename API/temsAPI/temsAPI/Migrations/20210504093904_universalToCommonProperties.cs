using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class universalToCommonProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UniversalProperties",
                table: "ReportTemplates",
                newName: "CommonProperties");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommonProperties",
                table: "ReportTemplates",
                newName: "UniversalProperties");
        }
    }
}
