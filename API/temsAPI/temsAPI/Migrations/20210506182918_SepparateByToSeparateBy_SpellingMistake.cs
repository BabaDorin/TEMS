using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class SepparateByToSeparateBy_SpellingMistake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SepparateBy",
                table: "ReportTemplates",
                newName: "SeparateBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeparateBy",
                table: "ReportTemplates",
                newName: "SepparateBy");
        }
    }
}
