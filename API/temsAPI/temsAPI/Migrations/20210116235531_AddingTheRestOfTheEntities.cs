using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddingTheRestOfTheEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentTypes_EquipmentTypeID1",
                table: "EquipmentSpecifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentSpecifications",
                table: "EquipmentSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentSpecifications_EquipmentTypeID1",
                table: "EquipmentSpecifications");

            migrationBuilder.DropColumn(
                name: "EquipmentTypeID1",
                table: "EquipmentSpecifications");

            migrationBuilder.AlterColumn<string>(
                name: "EquipmentTypeID",
                table: "EquipmentSpecifications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ID",
                table: "EquipmentSpecifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegisterDate",
                table: "Equipments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Equipments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Equipments",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedDate",
                table: "Equipments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<double>(
                name: "Commentary",
                table: "Equipments",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentDefinitionID",
                table: "Equipments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentSpecifications",
                table: "EquipmentSpecifications",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateClosed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorID = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    Copies = table.Column<int>(type: "int", nullable: true, defaultValueSql: "1")
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
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Floor = table.Column<int>(type: "int", nullable: true)
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
                    LogTypeID = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                name: "PersonnelRoomSupervisories",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PersonnelID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateSet = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                name: "IX_Equipments_EquipmentDefinitionID",
                table: "Equipments",
                column: "EquipmentDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_AuthorID",
                table: "Announcements",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_DateCreated",
                table: "Announcements",
                column: "DateCreated");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments",
                column: "EquipmentDefinitionID",
                principalTable: "EquipmentDefinitions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentSpecifications",
                column: "EquipmentTypeID",
                principalTable: "EquipmentTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentDefinitions_EquipmentDefinitionID",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentTypes_EquipmentTypeID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropTable(
                name: "Announcements");

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
                name: "Personnel");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentSpecifications",
                table: "EquipmentSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_EquipmentDefinitionID",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "EquipmentSpecifications");

            migrationBuilder.DropColumn(
                name: "EquipmentDefinitionID",
                table: "Equipments");

            migrationBuilder.AlterColumn<string>(
                name: "EquipmentTypeID",
                table: "EquipmentSpecifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentTypeID1",
                table: "EquipmentSpecifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegisterDate",
                table: "Equipments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Equipments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Equipments",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedDate",
                table: "Equipments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Commentary",
                table: "Equipments",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentSpecifications",
                table: "EquipmentSpecifications",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecifications_EquipmentTypeID1",
                table: "EquipmentSpecifications",
                column: "EquipmentTypeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentSpecifications_EquipmentTypes_EquipmentTypeID1",
                table: "EquipmentSpecifications",
                column: "EquipmentTypeID1",
                principalTable: "EquipmentTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
