using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAntiCheatAndSubjectSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add IsLocked (anti-cheat) to ExamTakerAssignments
            // NOTE: TabSwitchCount and LastTabSwitchAtUtc already exist from Phase25_GradingSchemaFix
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "ExamTakerAssignments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // 2. Add MaxTabSwitches to Assignments
            // NOTE: SubjectId and ClassLevelId already exist from Phase25/Phase26 migrations
            migrationBuilder.AddColumn<int>(
                name: "MaxTabSwitches",
                table: "Assignments",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            // 3. Add SubjectId to AssessmentModules (new)
            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "AssessmentModules",
                type: "uuid",
                nullable: true);

            // 4. Add SubjectId to Groups (new)
            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "Groups",
                type: "uuid",
                nullable: true);

            // 5. Create indices for new columns
            migrationBuilder.CreateIndex(
                name: "IX_Groups_SubjectId",
                table: "Groups",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentModules_SubjectId",
                table: "AssessmentModules",
                column: "SubjectId");

            // 6. Add Foreign Keys for new columns
            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentModules_Subjects_SubjectId",
                table: "AssessmentModules",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Subjects_SubjectId",
                table: "Groups",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentModules_Subjects_SubjectId",
                table: "AssessmentModules");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Subjects_SubjectId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_SubjectId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_AssessmentModules_SubjectId",
                table: "AssessmentModules");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "AssessmentModules");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "ExamTakerAssignments");

            migrationBuilder.DropColumn(
                name: "MaxTabSwitches",
                table: "Assignments");
        }
    }
}
