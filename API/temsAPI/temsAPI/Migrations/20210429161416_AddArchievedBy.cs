using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddArchievedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "Statuses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "RoomLabels",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "ReportTemplates",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "Properties",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "PersonnelRoomSupervisories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "PersonnelPositions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "Personnel",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "Logs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "Keys",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "KeyAllocations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "EquipmentTypes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "EquipmentSpecifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "Equipments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TEMSUserId",
                table: "Equipments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "EquipmentDefinitions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "EquipmentAllocations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchievedById",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ArchievedById",
                table: "Tickets",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_ArchievedById",
                table: "Statuses",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ArchievedById",
                table: "Rooms",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomLabels_ArchievedById",
                table: "RoomLabels",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_ArchievedById",
                table: "ReportTemplates",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ArchievedById",
                table: "Properties",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelRoomSupervisories_ArchievedById",
                table: "PersonnelRoomSupervisories",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelPositions_ArchievedById",
                table: "PersonnelPositions",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_ArchievedById",
                table: "Personnel",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ArchievedById",
                table: "Logs",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Keys_ArchievedById",
                table: "Keys",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_KeyAllocations_ArchievedById",
                table: "KeyAllocations",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_ArchievedById",
                table: "EquipmentTypes",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_ArchievedById",
                table: "EquipmentSpecifications",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ArchievedById",
                table: "Equipments",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_TEMSUserId",
                table: "Equipments",
                column: "TEMSUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_ArchievedById",
                table: "EquipmentDefinitions",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAllocations_ArchievedById",
                table: "EquipmentAllocations",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ArchievedById",
                table: "AspNetUsers",
                column: "ArchievedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ArchievedById",
                table: "AspNetUsers",
                column: "ArchievedById",
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
                name: "FK_Equipments_AspNetUsers_TEMSUserId",
                table: "Equipments",
                column: "TEMSUserId",
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
                name: "FK_Logs_AspNetUsers_ArchievedById",
                table: "Logs",
                column: "ArchievedById",
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
                name: "FK_PersonnelRoomSupervisories_AspNetUsers_ArchievedById",
                table: "PersonnelRoomSupervisories",
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
                name: "FK_ReportTemplates_AspNetUsers_ArchievedById",
                table: "ReportTemplates",
                column: "ArchievedById",
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
                name: "FK_Statuses_AspNetUsers_ArchievedById",
                table: "Statuses",
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ArchievedById",
                table: "AspNetUsers");

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
                name: "FK_Equipments_AspNetUsers_TEMSUserId",
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
                name: "FK_Logs_AspNetUsers_ArchievedById",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Personnel_AspNetUsers_ArchievedById",
                table: "Personnel");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelPositions_AspNetUsers_ArchievedById",
                table: "PersonnelPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_AspNetUsers_ArchievedById",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_ArchievedById",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplates_AspNetUsers_ArchievedById",
                table: "ReportTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomLabels_AspNetUsers_ArchievedById",
                table: "RoomLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_ArchievedById",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Statuses_AspNetUsers_ArchievedById",
                table: "Statuses");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ArchievedById",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ArchievedById",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Statuses_ArchievedById",
                table: "Statuses");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ArchievedById",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_RoomLabels_ArchievedById",
                table: "RoomLabels");

            migrationBuilder.DropIndex(
                name: "IX_ReportTemplates_ArchievedById",
                table: "ReportTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ArchievedById",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_PersonnelRoomSupervisories_ArchievedById",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropIndex(
                name: "IX_PersonnelPositions_ArchievedById",
                table: "PersonnelPositions");

            migrationBuilder.DropIndex(
                name: "IX_Personnel_ArchievedById",
                table: "Personnel");

            migrationBuilder.DropIndex(
                name: "IX_Logs_ArchievedById",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Keys_ArchievedById",
                table: "Keys");

            migrationBuilder.DropIndex(
                name: "IX_KeyAllocations_ArchievedById",
                table: "KeyAllocations");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentTypes_ArchievedById",
                table: "EquipmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentSpecifications_ArchievedById",
                table: "EquipmentSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_ArchievedById",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_TEMSUserId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentDefinitions_ArchievedById",
                table: "EquipmentDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentAllocations_ArchievedById",
                table: "EquipmentAllocations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ArchievedById",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "RoomLabels");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "ReportTemplates");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "PersonnelPositions");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "Personnel");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "Keys");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "KeyAllocations");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "EquipmentSpecifications");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "TEMSUserId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "EquipmentDefinitions");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "EquipmentAllocations");

            migrationBuilder.DropColumn(
                name: "ArchievedById",
                table: "AspNetUsers");
        }
    }
}
