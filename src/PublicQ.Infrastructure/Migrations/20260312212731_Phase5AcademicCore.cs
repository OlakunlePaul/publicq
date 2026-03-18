using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase5AcademicCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "ExamTakers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "ClassLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SectionOrArm = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Terms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextTermBegins = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextTermBills = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terms_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamTakerId = table.Column<string>(type: "text", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassLevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsLockedForParents = table.Column<bool>(type: "boolean", nullable: false),
                    TotalMarksObtained = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalMarksObtainable = table.Column<decimal>(type: "numeric", nullable: true),
                    AverageScore = table.Column<decimal>(type: "numeric", nullable: true),
                    PositionInClass = table.Column<int>(type: "integer", nullable: true),
                    NumberInClass = table.Column<int>(type: "integer", nullable: true),
                    OverallGrade = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    TimesSchoolOpened = table.Column<int>(type: "integer", nullable: true),
                    TimesPresent = table.Column<int>(type: "integer", nullable: true),
                    TimesAbsent = table.Column<int>(type: "integer", nullable: true),
                    Regularity = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Punctuality = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Neatness = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AttitudeInSchool = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    SocialActivities = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IndoorGames = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    FieldGames = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    TrackGames = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Jumps = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Swims = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ClassTeacherComment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    HeadTeacherComment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssessments_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAssessments_ExamTakers_ExamTakerId",
                        column: x => x.ExamTakerId,
                        principalTable: "ExamTakers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssessments_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAssessments_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentAssessmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestScore = table.Column<decimal>(type: "numeric", nullable: true),
                    ExamScore = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalScore = table.Column<decimal>(type: "numeric", nullable: true),
                    Grade = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    SubjectRemark = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectScores_StudentAssessments_StudentAssessmentId",
                        column: x => x.StudentAssessmentId,
                        principalTable: "StudentAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectScores_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevels_Name_SectionOrArm",
                table: "ClassLevels",
                columns: new[] { "Name", "SectionOrArm" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Name",
                table: "Sessions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessments_ClassLevelId",
                table: "StudentAssessments",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessments_ExamTakerId_SessionId_TermId",
                table: "StudentAssessments",
                columns: new[] { "ExamTakerId", "SessionId", "TermId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessments_SessionId",
                table: "StudentAssessments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessments_TermId",
                table: "StudentAssessments",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectScores_StudentAssessmentId_SubjectId",
                table: "SubjectScores",
                columns: new[] { "StudentAssessmentId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectScores_SubjectId",
                table: "SubjectScores",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Name",
                table: "Subjects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Terms_SessionId",
                table: "Terms",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectScores");

            migrationBuilder.DropTable(
                name: "StudentAssessments");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "ClassLevels");

            migrationBuilder.DropTable(
                name: "Terms");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "ExamTakers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
