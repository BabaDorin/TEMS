using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class OnDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentDefinitions_EquipmentDefinitionID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_Properties_PropertyID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypeKinships_EquipmentTypes_ParentEquipmentTypeId",
                table: "EquipmentTypeKinships");

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
                name: "FK_PersonnelEquipmentAllocations_Equipments_EquipmentID",
                table: "PersonnelEquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelEquipmentAllocations_Personnel_PersonnelID",
                table: "PersonnelEquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_Personnel_PersonnelID",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_Rooms_RoomID",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_DataTypes_DataTypeID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipmentAllocations_Equipments_EquipmentID",
                table: "RoomEquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipmentAllocations_Rooms_RoomID",
                table: "RoomEquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Equipments_EquipmentID",
                table: "Tickets");

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

            migrationBuilder.CreateTable(
                name: "EquipmentTypeProperty",
                columns: table => new
                {
                    EquipmentTypesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PropertiesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeProperty", x => new { x.EquipmentTypesId, x.PropertiesId });
                    table.ForeignKey(
                        name: "FK_EquipmentTypeProperty_EquipmentTypes_EquipmentTypesId",
                        column: x => x.EquipmentTypesId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeProperty_Properties_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeProperty_PropertiesId",
                table: "EquipmentTypeProperty",
                column: "PropertiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions",
                column: "EquipmentTypeID",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypeKinships_EquipmentTypes_ParentEquipmentTypeId",
                table: "EquipmentTypeKinships",
                column: "ParentEquipmentTypeId",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_PersonnelEquipmentAllocations_Equipments_EquipmentID",
                table: "PersonnelEquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelEquipmentAllocations_Personnel_PersonnelID",
                table: "PersonnelEquipmentAllocations",
                column: "PersonnelID",
                principalTable: "Personnel",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_DataTypes_DataTypeID",
                table: "Properties",
                column: "DataTypeID",
                principalTable: "DataTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipmentAllocations_Equipments_EquipmentID",
                table: "RoomEquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipmentAllocations_Rooms_RoomID",
                table: "RoomEquipmentAllocations",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Equipments_EquipmentID",
                table: "Tickets",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentDefinitions_EquipmentDefinitionID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_Properties_PropertyID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypeKinships_EquipmentTypes_ParentEquipmentTypeId",
                table: "EquipmentTypeKinships");

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
                name: "FK_PersonnelEquipmentAllocations_Equipments_EquipmentID",
                table: "PersonnelEquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelEquipmentAllocations_Personnel_PersonnelID",
                table: "PersonnelEquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_Personnel_PersonnelID",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelRoomSupervisories_Rooms_RoomID",
                table: "PersonnelRoomSupervisories");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_DataTypes_DataTypeID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipmentAllocations_Equipments_EquipmentID",
                table: "RoomEquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomEquipmentAllocations_Rooms_RoomID",
                table: "RoomEquipmentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Equipments_EquipmentID",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "EquipmentTypeProperty");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "EquipmentDefinitions");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "EquipmentDefinitions");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentDefinitions",
                column: "EquipmentTypeID",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypeKinships_EquipmentTypes_ParentEquipmentTypeId",
                table: "EquipmentTypeKinships",
                column: "ParentEquipmentTypeId",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_PersonnelEquipmentAllocations_Equipments_EquipmentID",
                table: "PersonnelEquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelEquipmentAllocations_Personnel_PersonnelID",
                table: "PersonnelEquipmentAllocations",
                column: "PersonnelID",
                principalTable: "Personnel",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_DataTypes_DataTypeID",
                table: "Properties",
                column: "DataTypeID",
                principalTable: "DataTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipmentAllocations_Equipments_EquipmentID",
                table: "RoomEquipmentAllocations",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomEquipmentAllocations_Rooms_RoomID",
                table: "RoomEquipmentAllocations",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Equipments_EquipmentID",
                table: "Tickets",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
