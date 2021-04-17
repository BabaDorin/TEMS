using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class ImplementOnDeleteCascade_Session2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Equipments_EquipmentID",
                table: "Logs");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Equipments_EquipmentID",
                table: "Logs",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Equipments_EquipmentID",
                table: "Logs");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Equipments_EquipmentID",
                table: "Logs",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
