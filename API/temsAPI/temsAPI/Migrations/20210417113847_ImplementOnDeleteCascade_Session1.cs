using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class ImplementOnDeleteCascade_Session1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_Personnel_PersonnelID",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_Rooms_RoomID",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyAllocations_Keys_KeyID",
                table: "KeyAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyAllocations_Personnel_PersonnelID",
                table: "KeyAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Equipments_EquipmentID",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_Personnel_PersonnelID",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_Rooms_RoomID",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Personnel_PersonnelID",
                table: "EquipmentAllocations",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Rooms_RoomID",
                table: "EquipmentAllocations",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments",
                column: "EquipmentDefinitionID",
                principalTable: "EquipmentDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_Keys_KeyID",
                table: "KeyAllocations",
                column: "KeyID",
                principalTable: "Keys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_Personnel_PersonnelID",
                table: "KeyAllocations",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Equipments_EquipmentID",
                table: "Logs",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelRoomSupervisories_Personnel_PersonnelID",
                table: "PersonnelRoomSupervisories",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelRoomSupervisories_Rooms_RoomID",
                table: "PersonnelRoomSupervisories",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_Personnel_PersonnelID",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_Rooms_RoomID",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyAllocations_Keys_KeyID",
                table: "KeyAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyAllocations_Personnel_PersonnelID",
                table: "KeyAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Equipments_EquipmentID",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_Personnel_PersonnelID",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_Rooms_RoomID",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                table: "EquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Personnel_PersonnelID",
                table: "EquipmentAllocations",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_Rooms_RoomID",
                table: "EquipmentAllocations",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments",
                column: "EquipmentDefinitionID",
                principalTable: "EquipmentDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_Keys_KeyID",
                table: "KeyAllocations",
                column: "KeyID",
                principalTable: "Keys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_Personnel_PersonnelID",
                table: "KeyAllocations",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Equipments_EquipmentID",
                table: "Logs",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelRoomSupervisories_Personnel_PersonnelID",
                table: "PersonnelRoomSupervisories",
                column: "PersonnelID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelRoomSupervisories_Rooms_RoomID",
                table: "PersonnelRoomSupervisories",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
