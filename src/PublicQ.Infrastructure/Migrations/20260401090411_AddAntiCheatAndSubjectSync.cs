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
            // 1. Add Anti-Cheat columns to StudentAssignments (ExamTakerAssignments)
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "ExamTakerAssignments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // 2. Add Anti-Cheat and Subject columns to Assignments
            migrationBuilder.AddColumn<int>(
                name: "MaxTabSwitches",
                table: "Assignments",
                type: "integer",
                nullable: false,
                defaultValue: 3);


            migrationBuilder.AddColumn<Guid>(
                name: "ClassLevelId",
                table: "Assignments",
                type: "uuid",
                nullable: true);

            // 3. Add SubjectId to AssessmentModules
            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "AssessmentModules",
                type: "uuid",
                nullable: true);

            // 4. Add SubjectId to Groups
            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "Groups",
                type: "uuid",
                nullable: true);

            // 5. Create Indices
            migrationBuilder.CreateIndex(
                name: "IX_Groups_SubjectId",
                table: "Groups",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentModules_SubjectId",
                table: "AssessmentModules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ClassLevelId",
                table: "Assignments",
                column: "ClassLevelId");


            // 6. Add Foreign Keys
            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentModules_Subjects_SubjectId",
                table: "AssessmentModules",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_ClassLevels_ClassLevelId",
                table: "Assignments",
                column: "ClassLevelId",
                principalTable: "ClassLevels",
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
                name: "FK_Assignments_ClassLevels_ClassLevelId",
                table: "Assignments");


            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Subjects_SubjectId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_SubjectId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_AssessmentModules_SubjectId",
                table: "AssessmentModules");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_ClassLevelId",
                table: "Assignments");


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
                name: "ClassLevelId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "MaxTabSwitches",
                table: "Assignments");

        }
    }
}
