using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddEquipmentLabelAttributeToLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Label",
                table: "EquipmentAllocations");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentLabel",
                table: "Logs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "Equipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentLabel",
                table: "EquipmentAllocations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentLabel",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "EquipmentLabel",
                table: "EquipmentAllocations");

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "EquipmentAllocations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
