using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class DescriptionAddedToDefinitionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Definition",
                table: "EquipmentDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EquipmentDefinitions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "EquipmentDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "Definition",
                table: "EquipmentDefinitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
