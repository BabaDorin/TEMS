using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class EquipmentDefinitionRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentDefinitions_EquipmentDefinitionID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_Properties_PropertyID",
                table: "EquipmentSpecifications");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "EquipmentDefinitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "EquipmentDefinitions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentDefinitions_EquipmentDefinitionID",
                table: "EquipmentSpecifications",
                column: "EquipmentDefinitionID",
                principalTable: "EquipmentDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_Properties_PropertyID",
                table: "EquipmentSpecifications",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentDefinitions_EquipmentDefinitionID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_Properties_PropertyID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "EquipmentDefinitions");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "EquipmentDefinitions");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentDefinitions_EquipmentDefinitionID",
                table: "EquipmentSpecifications",
                column: "EquipmentDefinitionID",
                principalTable: "EquipmentDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_Properties_PropertyID",
                table: "EquipmentSpecifications",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
