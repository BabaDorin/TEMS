using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddingEquipmentEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataTypes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TEMSID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Commentary = table.Column<double>(type: "float", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDefect = table.Column<bool>(type: "bit", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Equipments_Equipments_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Equipments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentEquipmentTypeID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentTypes_EquipmentTypes_ParentEquipmentTypeID",
                        column: x => x.ParentEquipmentTypeID,
                        principalTable: "EquipmentTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataTypeID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Properties_DataTypes_DataTypeID",
                        column: x => x.DataTypeID,
                        principalTable: "DataTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentDefinitions",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EquipmentTypeID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDefinitions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                        column: x => x.EquipmentTypeID,
                        principalTable: "EquipmentTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSpecifications",
                columns: table => new
                {
                    EquipmentTypeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentTypeID1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PropertyID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSpecifications", x => x.EquipmentTypeID);
                    table.ForeignKey(
                        name: "FK_EquipmentSpecifications_EquipmentTypes_EquipmentTypeID1",
                        column: x => x.EquipmentTypeID1,
                        principalTable: "EquipmentTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentSpecifications_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyEquipmentTypeAssociations",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TypeID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PropertyID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyEquipmentTypeAssociations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PropertyEquipmentTypeAssociations_EquipmentTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "EquipmentTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyEquipmentTypeAssociations_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentDefinitionKinships",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ChildDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDefinitionKinships", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitionKinships_EquipmentDefinitions_ChildDefinitionID",
                        column: x => x.ChildDefinitionID,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitionKinships_EquipmentDefinitions_ParentDefinitionID",
                        column: x => x.ParentDefinitionID,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitionKinships_ChildDefinitionID",
                table: "EquipmentDefinitionKinships",
                column: "ChildDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitionKinships_ParentDefinitionID",
                table: "EquipmentDefinitionKinships",
                column: "ParentDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_EquipmentTypeID",
                table: "EquipmentDefinitions",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_Identifier",
                table: "EquipmentDefinitions",
                column: "Identifier");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ParentID",
                table: "Equipments",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SerialNumber",
                table: "Equipments",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_TEMSID",
                table: "Equipments",
                column: "TEMSID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_EquipmentTypeID",
                table: "EquipmentSpecifications",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_EquipmentTypeID1",
                table: "EquipmentSpecifications",
                column: "EquipmentTypeID1");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_PropertyID",
                table: "EquipmentSpecifications",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_ParentEquipmentTypeID",
                table: "EquipmentTypes",
                column: "ParentEquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_DataTypeID",
                table: "Properties",
                column: "DataTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyEquipmentTypeAssociations_PropertyID",
                table: "PropertyEquipmentTypeAssociations",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyEquipmentTypeAssociations_TypeID",
                table: "PropertyEquipmentTypeAssociations",
                column: "TypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentDefinitionKinships");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "EquipmentSpecifications");

            migrationBuilder.DropTable(
                name: "PropertyEquipmentTypeAssociations");

            migrationBuilder.DropTable(
                name: "EquipmentDefinitions");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "DataTypes");
        }
    }
}
