using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorID",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_AspNetUsers_ArchievedById",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentDefinitions_AspNetUsers_ArchievedById",
                table: "EquipmentDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_AspNetUsers_ArchievedById",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_AspNetUsers_ArchievedById",
                table: "EquipmentSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypes_AspNetUsers_ArchievedById",
                table: "EquipmentTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyAllocations_AspNetUsers_ArchievedById",
                table: "KeyAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Keys_AspNetUsers_ArchievedById",
                table: "Keys");

            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_AspNetUsers_UploadedById",
                table: "LibraryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_ArchievedById",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_CreatedByID",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_AspNetUsers_ArchievedById",
                table: "Personnel");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelPositions_AspNetUsers_ArchievedById",
                table: "PersonnelPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_ArchievedById",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_GeneratedByID",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_ArchievedById",
                table: "ReportTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_CreatedById",
                table: "ReportTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomLabels_AspNetUsers_ArchievedById",
                table: "RoomLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_ArchievedById",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ArchievedById",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ClosedById",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatedById",
                table: "Tickets");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorID",
                table: "Announcements",
                column: "AuthorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_AspNetUsers_ArchievedById",
                table: "EquipmentAllocations",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentDefinitions_AspNetUsers_ArchievedById",
                table: "EquipmentDefinitions",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_ArchievedById",
                table: "Equipments",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_AspNetUsers_ArchievedById",
                table: "EquipmentSpecifications",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_AspNetUsers_ArchievedById",
                table: "EquipmentTypes",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_AspNetUsers_ArchievedById",
                table: "KeyAllocations",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Keys_AspNetUsers_ArchievedById",
                table: "Keys",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_AspNetUsers_UploadedById",
                table: "LibraryItems",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_ArchievedById",
                table: "Logs",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_CreatedByID",
                table: "Logs",
                column: "CreatedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personnel_AspNetUsers_ArchievedById",
                table: "Personnel",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelPositions_AspNetUsers_ArchievedById",
                table: "PersonnelPositions",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_ArchievedById",
                table: "Properties",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_GeneratedByID",
                table: "Reports",
                column: "GeneratedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_ArchievedById",
                table: "ReportTemplates",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_CreatedById",
                table: "ReportTemplates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomLabels_AspNetUsers_ArchievedById",
                table: "RoomLabels",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_ArchievedById",
                table: "Rooms",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ArchievedById",
                table: "Tickets",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ClosedById",
                table: "Tickets",
                column: "ClosedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatedById",
                table: "Tickets",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorID",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentAllocations_AspNetUsers_ArchievedById",
                table: "EquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentDefinitions_AspNetUsers_ArchievedById",
                table: "EquipmentDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_AspNetUsers_ArchievedById",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_AspNetUsers_ArchievedById",
                table: "EquipmentSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypes_AspNetUsers_ArchievedById",
                table: "EquipmentTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyAllocations_AspNetUsers_ArchievedById",
                table: "KeyAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Keys_AspNetUsers_ArchievedById",
                table: "Keys");

            migrationBuilder.DropForeignKey(
                name: "FK_LibraryItems_AspNetUsers_UploadedById",
                table: "LibraryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_ArchievedById",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_CreatedByID",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_AspNetUsers_ArchievedById",
                table: "Personnel");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelPositions_AspNetUsers_ArchievedById",
                table: "PersonnelPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_ArchievedById",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_GeneratedByID",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_ArchievedById",
                table: "ReportTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_CreatedById",
                table: "ReportTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomLabels_AspNetUsers_ArchievedById",
                table: "RoomLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_ArchievedById",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ArchievedById",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ClosedById",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatedById",
                table: "Tickets");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorID",
                table: "Announcements",
                column: "AuthorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentAllocations_AspNetUsers_ArchievedById",
                table: "EquipmentAllocations",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentDefinitions_AspNetUsers_ArchievedById",
                table: "EquipmentDefinitions",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_ArchievedById",
                table: "Equipments",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_AspNetUsers_ArchievedById",
                table: "EquipmentSpecifications",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_AspNetUsers_ArchievedById",
                table: "EquipmentTypes",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KeyAllocations_AspNetUsers_ArchievedById",
                table: "KeyAllocations",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Keys_AspNetUsers_ArchievedById",
                table: "Keys",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryItems_AspNetUsers_UploadedById",
                table: "LibraryItems",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_ArchievedById",
                table: "Logs",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_CreatedByID",
                table: "Logs",
                column: "CreatedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Personnel_AspNetUsers_ArchievedById",
                table: "Personnel",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelPositions_AspNetUsers_ArchievedById",
                table: "PersonnelPositions",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_ArchievedById",
                table: "Properties",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_GeneratedByID",
                table: "Reports",
                column: "GeneratedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_ArchievedById",
                table: "ReportTemplates",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_CreatedById",
                table: "ReportTemplates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomLabels_AspNetUsers_ArchievedById",
                table: "RoomLabels",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_ArchievedById",
                table: "Rooms",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ArchievedById",
                table: "Tickets",
                column: "ArchievedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ClosedById",
                table: "Tickets",
                column: "ClosedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_CreatedById",
                table: "Tickets",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
