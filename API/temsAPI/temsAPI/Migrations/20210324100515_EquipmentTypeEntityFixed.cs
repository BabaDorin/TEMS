using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class EquipmentTypeEntityFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypes_EquipmentTypes_ParentId",
                table: "EquipmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentTypes_ParentId",
                table: "EquipmentTypes");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "EquipmentTypes");

            migrationBuilder.CreateTable(
                name: "EquipmentTypeEquipmentType",
                columns: table => new
                {
                    ChildrenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeEquipmentType", x => new { x.ChildrenId, x.ParentsId });
                    table.ForeignKey(
                        name: "FK_EquipmentTypeEquipmentType_EquipmentTypes_ChildrenId",
                        column: x => x.ChildrenId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeEquipmentType_EquipmentTypes_ParentsId",
                        column: x => x.ParentsId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeEquipmentType_ParentsId",
                table: "EquipmentTypeEquipmentType",
                column: "ParentsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentTypeEquipmentType");

            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "EquipmentTypes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes_ParentId",
                table: "EquipmentTypes",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_EquipmentTypes_ParentId",
                table: "EquipmentTypes",
                column: "ParentId",
                principalTable: "EquipmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
