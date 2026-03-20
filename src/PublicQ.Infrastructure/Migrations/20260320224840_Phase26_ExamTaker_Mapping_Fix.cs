using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase26_ExamTaker_Mapping_Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClassLevelId",
                table: "Assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassLevelSubjects",
                columns: table => new
                {
                    ClassLevelsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLevelSubjects", x => new { x.ClassLevelsId, x.SubjectsId });
                    table.ForeignKey(
                        name: "FK_ClassLevelSubjects_ClassLevels_ClassLevelsId",
                        column: x => x.ClassLevelsId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassLevelSubjects_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ClassLevelId",
                table: "Assignments",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelSubjects_SubjectsId",
                table: "ClassLevelSubjects",
                column: "SubjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_ClassLevels_ClassLevelId",
                table: "Assignments",
                column: "ClassLevelId",
                principalTable: "ClassLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_ClassLevels_ClassLevelId",
                table: "Assignments");

            migrationBuilder.DropTable(
                name: "ClassLevelSubjects");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_ClassLevelId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "ClassLevelId",
                table: "Assignments");
        }
    }
}
