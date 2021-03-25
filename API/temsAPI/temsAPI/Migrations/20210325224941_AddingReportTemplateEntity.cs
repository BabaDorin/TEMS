using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class AddingReportTemplateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SepparateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Header = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Footer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentDefinitionReportTemplate",
                columns: table => new
                {
                    EquipmentDefinitionsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReportTemplatesMemberOfId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDefinitionReportTemplate", x => new { x.EquipmentDefinitionsId, x.ReportTemplatesMemberOfId });
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitionReportTemplate_EquipmentDefinitions_EquipmentDefinitionsId",
                        column: x => x.EquipmentDefinitionsId,
                        principalTable: "EquipmentDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentDefinitionReportTemplate_ReportTemplates_ReportTemplatesMemberOfId",
                        column: x => x.ReportTemplatesMemberOfId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypeReportTemplate",
                columns: table => new
                {
                    EquipmentTypesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReportTemplatesMemberOfId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeReportTemplate", x => new { x.EquipmentTypesId, x.ReportTemplatesMemberOfId });
                    table.ForeignKey(
                        name: "FK_EquipmentTypeReportTemplate_EquipmentTypes_EquipmentTypesId",
                        column: x => x.EquipmentTypesId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTypeReportTemplate_ReportTemplates_ReportTemplatesMemberOfId",
                        column: x => x.ReportTemplatesMemberOfId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelReportTemplate",
                columns: table => new
                {
                    ReportTemplatesAssignedId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SignatoriesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelReportTemplate", x => new { x.ReportTemplatesAssignedId, x.SignatoriesId });
                    table.ForeignKey(
                        name: "FK_PersonnelReportTemplate_Personnel_SignatoriesId",
                        column: x => x.SignatoriesId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelReportTemplate_ReportTemplates_ReportTemplatesAssignedId",
                        column: x => x.ReportTemplatesAssignedId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelReportTemplate1",
                columns: table => new
                {
                    PersonnelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReportTemplatesMemberId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelReportTemplate1", x => new { x.PersonnelId, x.ReportTemplatesMemberId });
                    table.ForeignKey(
                        name: "FK_PersonnelReportTemplate1_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelReportTemplate1_ReportTemplates_ReportTemplatesMemberId",
                        column: x => x.ReportTemplatesMemberId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyReportTemplate",
                columns: table => new
                {
                    PropertiesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReportTemplatesMemberOfId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyReportTemplate", x => new { x.PropertiesId, x.ReportTemplatesMemberOfId });
                    table.ForeignKey(
                        name: "FK_PropertyReportTemplate_Properties_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyReportTemplate_ReportTemplates_ReportTemplatesMemberOfId",
                        column: x => x.ReportTemplatesMemberOfId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplateRoom",
                columns: table => new
                {
                    ReportTemplatesMemberOfId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplateRoom", x => new { x.ReportTemplatesMemberOfId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_ReportTemplateRoom_ReportTemplates_ReportTemplatesMemberOfId",
                        column: x => x.ReportTemplatesMemberOfId,
                        principalTable: "ReportTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportTemplateRoom_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentDefinitionReportTemplate_ReportTemplatesMemberOfId",
                table: "EquipmentDefinitionReportTemplate",
                column: "ReportTemplatesMemberOfId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypeReportTemplate_ReportTemplatesMemberOfId",
                table: "EquipmentTypeReportTemplate",
                column: "ReportTemplatesMemberOfId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelReportTemplate_SignatoriesId",
                table: "PersonnelReportTemplate",
                column: "SignatoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelReportTemplate1_ReportTemplatesMemberId",
                table: "PersonnelReportTemplate1",
                column: "ReportTemplatesMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyReportTemplate_ReportTemplatesMemberOfId",
                table: "PropertyReportTemplate",
                column: "ReportTemplatesMemberOfId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplateRoom_RoomsId",
                table: "ReportTemplateRoom",
                column: "RoomsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentDefinitionReportTemplate");

            migrationBuilder.DropTable(
                name: "EquipmentTypeReportTemplate");

            migrationBuilder.DropTable(
                name: "PersonnelReportTemplate");

            migrationBuilder.DropTable(
                name: "PersonnelReportTemplate1");

            migrationBuilder.DropTable(
                name: "PropertyReportTemplate");

            migrationBuilder.DropTable(
                name: "ReportTemplateRoom");

            migrationBuilder.DropTable(
                name: "ReportTemplates");
        }
    }
}
