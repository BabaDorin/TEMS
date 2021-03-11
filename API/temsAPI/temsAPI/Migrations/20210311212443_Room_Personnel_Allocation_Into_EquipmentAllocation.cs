using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class Room_Personnel_Allocation_Into_EquipmentAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelEquipmentAllocations");

            migrationBuilder.DropTable(
                name: "RoomEquipmentAllocations");

            migrationBuilder.CreateTable(
                name: "EquipmentAllocations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateAllocated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersonnelID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentAllocations_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentAllocations_Personnel_PersonnelID",
                        column: x => x.PersonnelID,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentAllocations_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAllocations_EquipmentID",
                table: "EquipmentAllocations",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAllocations_PersonnelID",
                table: "EquipmentAllocations",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAllocations_RoomID",
                table: "EquipmentAllocations",
                column: "RoomID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentAllocations");

            migrationBuilder.CreateTable(
                name: "PersonnelEquipmentAllocations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateAllocated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EquipmentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PersonnelID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelEquipmentAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelEquipmentAllocations_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelEquipmentAllocations_Personnel_PersonnelID",
                        column: x => x.PersonnelID,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomEquipmentAllocations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateAllocated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EquipmentID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomEquipmentAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomEquipmentAllocations_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomEquipmentAllocations_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEquipmentAllocations_EquipmentID",
                table: "PersonnelEquipmentAllocations",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelEquipmentAllocations_PersonnelID",
                table: "PersonnelEquipmentAllocations",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipmentAllocations_EquipmentID",
                table: "RoomEquipmentAllocations",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipmentAllocations_RoomID",
                table: "RoomEquipmentAllocations",
                column: "RoomID");
        }
    }
}
