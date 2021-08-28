using Microsoft.EntityFrameworkCore.Migrations;

namespace temsAPI.Migrations
{
    public partial class NoPersonnelReferenceOnSignatories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelReportTemplate_Personnel_SignatoriesId",
                table: "PersonnelReportTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelReportTemplate_ReportTemplates_ReportTemplatesAssignedId",
                table: "PersonnelReportTemplate");

            migrationBuilder.DropTable(
                name: "PersonnelReportTemplate1");

            migrationBuilder.RenameColumn(
                name: "SignatoriesId",
                table: "PersonnelReportTemplate",
                newName: "ReportTemplatesMemberId");

            migrationBuilder.RenameColumn(
                name: "ReportTemplatesAssignedId",
                table: "PersonnelReportTemplate",
                newName: "PersonnelId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonnelReportTemplate_SignatoriesId",
                table: "PersonnelReportTemplate",
                newName: "IX_PersonnelReportTemplate_ReportTemplatesMemberId");

            migrationBuilder.AddColumn<string>(
                name: "Signatories",
                table: "ReportTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelReportTemplate_Personnel_PersonnelId",
                table: "PersonnelReportTemplate",
                column: "PersonnelId",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelReportTemplate_ReportTemplates_ReportTemplatesMemberId",
                table: "PersonnelReportTemplate",
                column: "ReportTemplatesMemberId",
                principalTable: "ReportTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelReportTemplate_Personnel_PersonnelId",
                table: "PersonnelReportTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonnelReportTemplate_ReportTemplates_ReportTemplatesMemberId",
                table: "PersonnelReportTemplate");

            migrationBuilder.DropColumn(
                name: "Signatories",
                table: "ReportTemplates");

            migrationBuilder.RenameColumn(
                name: "ReportTemplatesMemberId",
                table: "PersonnelReportTemplate",
                newName: "SignatoriesId");

            migrationBuilder.RenameColumn(
                name: "PersonnelId",
                table: "PersonnelReportTemplate",
                newName: "ReportTemplatesAssignedId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonnelReportTemplate_ReportTemplatesMemberId",
                table: "PersonnelReportTemplate",
                newName: "IX_PersonnelReportTemplate_SignatoriesId");

            migrationBuilder.CreateTable(
                name: "PersonnelReportTemplate1",
                columns: table => new
                {
                    PersonnelId = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    ReportTemplatesMemberId = table.Column<string>(type: "nvarchar(150)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelReportTemplate1_ReportTemplatesMemberId",
                table: "PersonnelReportTemplate1",
                column: "ReportTemplatesMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelReportTemplate_Personnel_SignatoriesId",
                table: "PersonnelReportTemplate",
                column: "SignatoriesId",
                principalTable: "Personnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonnelReportTemplate_ReportTemplates_ReportTemplatesAssignedId",
                table: "PersonnelReportTemplate",
                column: "ReportTemplatesAssignedId",
                principalTable: "ReportTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
