using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class newRoomSupervisoriesApproach : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelRoomSupervisories");

            migrationBuilder.CreateTable(
                name: "PersonnelRoom",
                columns: table => new
                {
                    RoomsSupervisoriedId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SupervisoriesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelRoom", x => new { x.RoomsSupervisoriedId, x.SupervisoriesId });
                    table.ForeignKey(
                        name: "FK_PersonnelRoom_Personnel_SupervisoriesId",
                        column: x => x.SupervisoriesId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelRoom_Rooms_RoomsSupervisoriedId",
                        column: x => x.RoomsSupervisoriedId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelRoom_SupervisoriesId",
                table: "PersonnelRoom",
                column: "SupervisoriesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelRoom");

            migrationBuilder.CreateTable(
                name: "PersonnelRoomSupervisories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ArchievedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateArchieved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchieved = table.Column<bool>(type: "bit", nullable: false),
                    PersonnelID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelRoomSupervisories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelRoomSupervisories_AspNetUsers_ArchievedById",
                        column: x => x.ArchievedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelRoomSupervisories_Personnel_PersonnelID",
                        column: x => x.PersonnelID,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonnelRoomSupervisories_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelRoomSupervisories_ArchievedById",
                table: "PersonnelRoomSupervisories",
                column: "ArchievedById");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelRoomSupervisories_PersonnelID",
                table: "PersonnelRoomSupervisories",
                column: "PersonnelID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelRoomSupervisories_RoomID",
                table: "PersonnelRoomSupervisories",
                column: "RoomID");
        }
    }
}
