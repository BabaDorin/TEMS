using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class ticket_addPreviouslyClosedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TEMSUserTicket1",
                columns: table => new
                {
                    ClosedAndThenReopenedTicketsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PreviouslyClosedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEMSUserTicket1", x => new { x.ClosedAndThenReopenedTicketsId, x.PreviouslyClosedById });
                    table.ForeignKey(
                        name: "FK_TEMSUserTicket1_AspNetUsers_PreviouslyClosedById",
                        column: x => x.PreviouslyClosedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TEMSUserTicket1_Tickets_ClosedAndThenReopenedTicketsId",
                        column: x => x.ClosedAndThenReopenedTicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TEMSUserTicket1_PreviouslyClosedById",
                table: "TEMSUserTicket1",
                column: "PreviouslyClosedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TEMSUserTicket1");
        }
    }
}
