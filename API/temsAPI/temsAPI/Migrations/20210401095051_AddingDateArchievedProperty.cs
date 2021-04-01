using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddingDateArchievedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCanceled",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.RenameColumn(
                name: "DateSet",
                table: "PersonnelRoomSupervisories",
                newName: "DateArchieved");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "Statuses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "Rooms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "RoomLabels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "ReportTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "Properties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "PersonnelPositions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "Personnel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "Keys",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "KeyAllocations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "EquipmentTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "EquipmentSpecifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "Equipments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "EquipmentDefinitions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "EquipmentAllocations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchieved",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "RoomLabels");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "PersonnelPositions");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "Personnel");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "Keys");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "KeyAllocations");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "EquipmentSpecifications");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "EquipmentDefinitions");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "EquipmentAllocations");

            migrationBuilder.DropColumn(
                name: "DateArchieved",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "DateArchieved",
                table: "PersonnelRoomSupervisories",
                newName: "DateSet");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCanceled",
                table: "PersonnelRoomSupervisories",
                type: "datetime2",
                nullable: true);
        }
    }
}
