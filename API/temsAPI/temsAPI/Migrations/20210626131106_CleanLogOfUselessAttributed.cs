using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class CleanLogOfUselessAttributed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_LogTypes_LogTypeID",
                table: "Logs");

            migrationBuilder.DropTable(
                name: "LogTypes");

            migrationBuilder.DropIndex(
                name: "IX_Logs_LogTypeID",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "LogTypeID",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Logs",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Logs",
                newName: "Text");

            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Logs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LogTypeID",
                table: "Logs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LogTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_LogTypeID",
                table: "Logs",
                column: "LogTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_LogTypes_LogTypeID",
                table: "Logs",
                column: "LogTypeID",
                principalTable: "LogTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
