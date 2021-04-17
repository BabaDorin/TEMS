using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class fixPersonnelUserReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TEMSUserId",
                table: "Personnel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TEMSUserId",
                table: "Personnel",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
