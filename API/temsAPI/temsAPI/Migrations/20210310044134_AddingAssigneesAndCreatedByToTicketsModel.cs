using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddingAssigneesAndCreatedByToTicketsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TEMSUserTicket",
                columns: table => new
                {
                    AssignedTicketsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssigneesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEMSUserTicket", x => new { x.AssignedTicketsId, x.AssigneesId });
                    table.ForeignKey(
                        name: "FK_TEMSUserTicket_AspNetUsers_AssigneesId",
                        column: x => x.AssigneesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TEMSUserTicket_Tickets_AssignedTicketsId",
                        column: x => x.AssignedTicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedById",
                table: "Tickets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TEMSUserTicket_AssigneesId",
                table: "TEMSUserTicket",
                column: "AssigneesId");

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
                name: "FK_Tickets_AspNetUsers_CreatedById",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TEMSUserTicket");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CreatedById",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Tickets");
        }
    }
}
