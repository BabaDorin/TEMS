using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class ManualTypeDefinitionEquipmentDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions",
                column: "EquipmentTypeID",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments",
                column: "EquipmentDefinitionID",
                principalTable: "EquipmentDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions",
                column: "EquipmentTypeID",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments",
                column: "EquipmentDefinitionID",
                principalTable: "EquipmentDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
