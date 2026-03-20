using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase25_GradingSchemaFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastTabSwitchAtUtc",
                table: "ExamTakerAssignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TabSwitchCount",
                table: "ExamTakerAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "GradingSchemaId",
                table: "ClassLevels",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "Assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GradingSchemas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradingSchemas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GradeRanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GradingSchemaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MinScore = table.Column<int>(type: "integer", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false),
                    Remark = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeRanges_GradingSchemas_GradingSchemaId",
                        column: x => x.GradingSchemaId,
                        principalTable: "GradingSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Use raw SQL for idempotent insert/upsert to avoid unique constraint violations
            migrationBuilder.Sql(@"
                INSERT INTO ""SystemSettings"" (""Name"", ""Value"")
                VALUES 
                    ('EmailOptions:FrontendUrl', 'https://examnova.vercel.app'),
                    ('ResendOptions:ApiKey', '<Your Resend API Key>')
                ON CONFLICT (""Name"") DO UPDATE SET ""Value"" = EXCLUDED.""Value"";
            ");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevels_GradingSchemaId",
                table: "ClassLevels",
                column: "GradingSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_SubjectId",
                table: "Assignments",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeRanges_GradingSchemaId_Symbol",
                table: "GradeRanges",
                columns: new[] { "GradingSchemaId", "Symbol" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Subjects_SubjectId",
                table: "Assignments",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevels_GradingSchemas_GradingSchemaId",
                table: "ClassLevels",
                column: "GradingSchemaId",
                principalTable: "GradingSchemas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Subjects_SubjectId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevels_GradingSchemas_GradingSchemaId",
                table: "ClassLevels");

            migrationBuilder.DropTable(
                name: "GradeRanges");

            migrationBuilder.DropTable(
                name: "GradingSchemas");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevels_GradingSchemaId",
                table: "ClassLevels");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_SubjectId",
                table: "Assignments");

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Name",
                keyValue: "EmailOptions:FrontendUrl");

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Name",
                keyValue: "ResendOptions:ApiKey");

            migrationBuilder.DropColumn(
                name: "LastTabSwitchAtUtc",
                table: "ExamTakerAssignments");

            migrationBuilder.DropColumn(
                name: "TabSwitchCount",
                table: "ExamTakerAssignments");

            migrationBuilder.DropColumn(
                name: "GradingSchemaId",
                table: "ClassLevels");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Assignments");
        }
    }
}
