using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class EquipmentRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentTypeKinships");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "EquipmentTypeKinships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChildEquipmentTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParentEquipmentTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeKinships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeKinships_EquipmentTypes_ChildEquipmentTypeId",
                        column: x => x.ChildEquipmentTypeId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeKinships_EquipmentTypes_ParentEquipmentTypeId",
                        column: x => x.ParentEquipmentTypeId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeKinships_ChildEquipmentTypeId",
                table: "EquipmentTypeKinships",
                column: "ChildEquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeKinships_ParentEquipmentTypeId",
                table: "EquipmentTypeKinships",
                column: "ParentEquipmentTypeId");
        }
    }
}
