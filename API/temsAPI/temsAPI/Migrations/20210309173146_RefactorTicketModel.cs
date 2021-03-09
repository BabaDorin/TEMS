using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class RefactorTicketModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ClosedByID",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Equipments_EquipmentID",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Personnel_AuthorID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "EquipmentID",
                table: "Tickets",
                newName: "EquipmentId");

            migrationBuilder.RenameColumn(
                name: "ClosedByID",
                table: "Tickets",
                newName: "ClosedById");

            migrationBuilder.RenameColumn(
                name: "AuthorID",
                table: "Tickets",
                newName: "PersonnelId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_EquipmentID",
                table: "Tickets",
                newName: "IX_Tickets_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_ClosedByID",
                table: "Tickets",
                newName: "IX_Tickets_ClosedById");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_AuthorID",
                table: "Tickets",
                newName: "IX_Tickets_PersonnelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ClosedById",
                table: "Tickets",
                column: "ClosedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ClosedById",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Equipments_EquipmentId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Personnel_PersonnelId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "Tickets",
                newName: "EquipmentID");

            migrationBuilder.RenameColumn(
                name: "ClosedById",
                table: "Tickets",
                newName: "ClosedByID");

            migrationBuilder.RenameColumn(
                name: "PersonnelId",
                table: "Tickets",
                newName: "AuthorID");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_EquipmentId",
                table: "Tickets",
                newName: "IX_Tickets_EquipmentID");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_ClosedById",
                table: "Tickets",
                newName: "IX_Tickets_ClosedByID");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_PersonnelId",
                table: "Tickets",
                newName: "IX_Tickets_AuthorID");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ClosedByID",
                table: "Tickets",
                column: "ClosedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Equipments_EquipmentID",
                table: "Tickets",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Personnel_AuthorID",
                table: "Tickets",
                column: "AuthorID",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
