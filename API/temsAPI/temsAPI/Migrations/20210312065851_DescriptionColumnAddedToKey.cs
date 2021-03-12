using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class DescriptionColumnAddedToKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Copies",
                table: "Keys",
                type: "int",
                nullable: false,
                defaultValueSql: "1",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "1");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Keys",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Keys");

            migrationBuilder.AlterColumn<int>(
                name: "Copies",
                table: "Keys",
                type: "int",
                nullable: true,
                defaultValueSql: "1",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "1");
        }
    }
}
