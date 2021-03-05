using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class ManyToManyExperimenting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentTypeProperty",
                columns: table => new
                {
                    EquipmentTypesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PropertiesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeProperty", x => new { x.EquipmentTypesId, x.PropertiesId });
                    table.ForeignKey(
                        name: "FK_EquipmentTypeProperty_EquipmentTypes_EquipmentTypesId",
                        column: x => x.EquipmentTypesId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeProperty_Properties_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeProperty_PropertiesId",
                table: "EquipmentTypeProperty",
                column: "PropertiesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentTypeProperty");
        }
    }
}
