using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddingTEMSEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchieved",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateClosed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Announcements_AspNetUsers_AuthorID",
                        column: x => x.AuthorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "EquipmentTypes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentEquipmentTypeID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
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
                name: "FrequentTicketProblems",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Problem = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequentTicketProblems", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Keys",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Copies = table.Column<int>(type: "int", nullable: true, defaultValueSql: "1"),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keys", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LogTypes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Personnel",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Floor = table.Column<int>(type: "int", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ToDos",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateClosed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Urgent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToDos_AspNetUsers_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
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
                    EquipmentTypeID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDefinitions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitions_EquipmentDefinitions_ParentID",
                        column: x => x.ParentID,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitions_EquipmentTypes_EquipmentTypeID",
                        column: x => x.EquipmentTypeID,
                        principalTable: "EquipmentTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KeyAllocations",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PersonnelID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    KeyID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateAllocated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyAllocations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_KeyAllocations_Keys_KeyID",
                        column: x => x.KeyID,
                        principalTable: "Keys",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KeyAllocations_Personnel_PersonnelID",
                        column: x => x.PersonnelID,
                        principalTable: "Personnel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePrivileges",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PrivilegeID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePrivileges", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RolePrivileges_AspNetRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePrivileges_Privileges_PrivilegeID",
                        column: x => x.PrivilegeID,
                        principalTable: "Privileges",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelRoomSupervisories",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PersonnelID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateSet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCanceled = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelRoomSupervisories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PersonnelRoomSupervisories_Personnel_PersonnelID",
                        column: x => x.PersonnelID,
                        principalTable: "Personnel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelRoomSupervisories_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
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
                name: "Equipments",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TEMSID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Commentary = table.Column<double>(type: "float", nullable: true),
                    EquipmentDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RegisteredByID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefect = table.Column<bool>(type: "bit", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Equipments_AspNetUsers_RegisteredByID",
                        column: x => x.RegisteredByID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                        column: x => x.EquipmentDefinitionID,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_Equipments_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Equipments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSpecifications",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PropertyID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentSpecifications_EquipmentDefinitions_EquipmentDefinitionID",
                        column: x => x.EquipmentDefinitionID,
                        principalTable: "EquipmentDefinitions",
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
                name: "Logs",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquipmentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PersonnelID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsImportant = table.Column<bool>(type: "bit", nullable: false),
                    LogTypeID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Logs_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logs_LogTypes_LogTypeID",
                        column: x => x.LogTypeID,
                        principalTable: "LogTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logs_Personnel_PersonnelID",
                        column: x => x.PersonnelID,
                        principalTable: "Personnel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logs_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelEquipmentAllocations",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PersonnelID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateAllocated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelEquipmentAllocations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PersonnelEquipmentAllocations_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelEquipmentAllocations_Personnel_PersonnelID",
                        column: x => x.PersonnelID,
                        principalTable: "Personnel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomEquipmentAllocations",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateAllocated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomEquipmentAllocations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RoomEquipmentAllocations_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomEquipmentAllocations_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateClosed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosedByID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Problem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquipmentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_ClosedByID",
                        column: x => x.ClosedByID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Personnel_AuthorID",
                        column: x => x.AuthorID,
                        principalTable: "Personnel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_AuthorID",
                table: "Announcements",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_DateCreated",
                table: "Announcements",
                column: "DateCreated");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_EquipmentTypeID",
                table: "EquipmentDefinitions",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_Identifier",
                table: "EquipmentDefinitions",
                column: "Identifier");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitions_ParentID",
                table: "EquipmentDefinitions",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentDefinitionID",
                table: "Equipments",
                column: "EquipmentDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ParentID",
                table: "Equipments",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_RegisteredByID",
                table: "Equipments",
                column: "RegisteredByID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SerialNumber",
                table: "Equipments",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_TEMSID",
                table: "Equipments",
                column: "TEMSID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_EquipmentDefinitionID",
                table: "EquipmentSpecifications",
                column: "EquipmentDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_PropertyID",
                table: "EquipmentSpecifications",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_ParentEquipmentTypeID",
                table: "EquipmentTypes",
                column: "ParentEquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_KeyAllocations_KeyID",
                table: "KeyAllocations",
                column: "KeyID");

            migrationBuilder.CreateIndex(
                name: "IX_KeyAllocations_PersonnelID",
                table: "KeyAllocations",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_EquipmentID",
                table: "Logs",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_LogTypeID",
                table: "Logs",
                column: "LogTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_PersonnelID",
                table: "Logs",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_RoomID",
                table: "Logs",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEquipmentAllocations_EquipmentID",
                table: "PersonnelEquipmentAllocations",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEquipmentAllocations_PersonnelID",
                table: "PersonnelEquipmentAllocations",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelRoomSupervisories_PersonnelID",
                table: "PersonnelRoomSupervisories",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelRoomSupervisories_RoomID",
                table: "PersonnelRoomSupervisories",
                column: "RoomID");

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

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivileges_PrivilegeID",
                table: "RolePrivileges",
                column: "PrivilegeID");

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivileges_RoleID",
                table: "RolePrivileges",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipmentAllocations_EquipmentID",
                table: "RoomEquipmentAllocations",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipmentAllocations_RoomID",
                table: "RoomEquipmentAllocations",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AuthorID",
                table: "Tickets",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ClosedByID",
                table: "Tickets",
                column: "ClosedByID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EquipmentID",
                table: "Tickets",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RoomID",
                table: "Tickets",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDos_CreatedByID",
                table: "ToDos",
                column: "CreatedByID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "EquipmentSpecifications");

            migrationBuilder.DropTable(
                name: "FrequentTicketProblems");

            migrationBuilder.DropTable(
                name: "KeyAllocations");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "PersonnelEquipmentAllocations");

            migrationBuilder.DropTable(
                name: "PersonnelRoomSupervisories");

            migrationBuilder.DropTable(
                name: "PropertyEquipmentTypeAssociations");

            migrationBuilder.DropTable(
                name: "RolePrivileges");

            migrationBuilder.DropTable(
                name: "RoomEquipmentAllocations");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "ToDos");

            migrationBuilder.DropTable(
                name: "Keys");

            migrationBuilder.DropTable(
                name: "LogTypes");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Privileges");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Personnel");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "DataTypes");

            migrationBuilder.DropTable(
                name: "EquipmentDefinitions");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsArchieved",
                table: "AspNetUsers");
        }
    }
}
