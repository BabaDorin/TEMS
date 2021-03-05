using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class propertyRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Properties",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "Required",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Required",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Properties",
                newName: "ID");
        }
    }
}
