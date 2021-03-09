using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class RefactoringTicketEntity_ManyPersonnelEquipmentsRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Equipments_EquipmentId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Personnel_PersonnelId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Rooms_RoomID",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_EquipmentId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PersonnelId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_RoomID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PersonnelId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "Tickets");

            migrationBuilder.CreateTable(
                name: "EquipmentTicket",
                columns: table => new
                {
                    EquipmentsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTicket", x => new { x.EquipmentsId, x.TicketsId });
                    table.ForeignKey(
                        name: "FK_EquipmentTicket_Equipments_EquipmentsId",
                        column: x => x.EquipmentsId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTicket_Tickets_TicketsId",
                        column: x => x.TicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelTicket",
                columns: table => new
                {
                    PersonnelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelTicket", x => new { x.PersonnelId, x.TicketsId });
                    table.ForeignKey(
                        name: "FK_PersonnelTicket_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelTicket_Tickets_TicketsId",
                        column: x => x.TicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomTicket",
                columns: table => new
                {
                    RoomsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTicket", x => new { x.RoomsId, x.TicketsId });
                    table.ForeignKey(
                        name: "FK_RoomTicket_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomTicket_Tickets_TicketsId",
                        column: x => x.TicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTicket_TicketsId",
                table: "EquipmentTicket",
                column: "TicketsId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelTicket_TicketsId",
                table: "PersonnelTicket",
                column: "TicketsId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTicket_TicketsId",
                table: "RoomTicket",
                column: "TicketsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentTicket");

            migrationBuilder.DropTable(
                name: "PersonnelTicket");

            migrationBuilder.DropTable(
                name: "RoomTicket");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonnelId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomID",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EquipmentId",
                table: "Tickets",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PersonnelId",
                table: "Tickets",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RoomID",
                table: "Tickets",
                column: "RoomID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Equipments_EquipmentId",
                table: "Tickets",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Personnel_PersonnelId",
                table: "Tickets",
                column: "PersonnelId",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Rooms_RoomID",
                table: "Tickets",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
