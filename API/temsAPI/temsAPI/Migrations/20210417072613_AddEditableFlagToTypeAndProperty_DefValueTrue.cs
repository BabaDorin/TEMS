using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddEditableFlagToTypeAndProperty_DefValueTrue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EditablePropertyInfo",
                table: "Properties",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EditableTypeInfo",
                table: "EquipmentTypes",
                type: "bit",
                nullable: true,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditablePropertyInfo",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "EditableTypeInfo",
                table: "EquipmentTypes");
        }
    }
}
