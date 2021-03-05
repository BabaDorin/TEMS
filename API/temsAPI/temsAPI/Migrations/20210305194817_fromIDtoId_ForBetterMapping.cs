using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class fromIDtoId_ForBetterMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ToDos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Tickets",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Rooms",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "RoomEquipmentAllocations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "RolePrivileges",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PropertyEquipmentTypeAssociations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Privileges",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PersonnelRoomSupervisories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PersonnelEquipmentAllocations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Personnel",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "LogTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Logs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Keys",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "KeyAllocations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "FrequentTicketProblems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "EquipmentTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "EquipmentSpecifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Equipments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "EquipmentDefinitions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "DataTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Announcements",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ToDos",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tickets",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Rooms",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RoomEquipmentAllocations",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RolePrivileges",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PropertyEquipmentTypeAssociations",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Privileges",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PersonnelRoomSupervisories",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PersonnelEquipmentAllocations",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Personnel",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LogTypes",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Logs",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Keys",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "KeyAllocations",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FrequentTicketProblems",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EquipmentTypes",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EquipmentSpecifications",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Equipments",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EquipmentDefinitions",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DataTypes",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Announcements",
                newName: "ID");
        }
    }
}
