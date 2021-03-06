using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class Equipment_DescriptionFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Commentary",
                table: "Equipments");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Equipments");

            migrationBuilder.AddColumn<double>(
                name: "Commentary",
                table: "Equipments",
                type: "float",
                nullable: true);
        }
    }
}
