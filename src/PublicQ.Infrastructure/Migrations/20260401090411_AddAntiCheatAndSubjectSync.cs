using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAntiCheatAndSubjectSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentModuleVersions_AssessmentModules_AssessmentModule~",
                table: "AssessmentModuleVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentQuestions_AssessmentModuleVersions_AssessmentModu~",
                table: "AssessmentQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleProgress_AssessmentModuleVersions_AssessmentModuleVer~",
                table: "ModuleProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_PossibleAnswerEntityStaticFileEntity_AssessmentPossibleAnsw~",
                table: "PossibleAnswerEntityStaticFileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_PossibleAnswerEntityStaticFileEntity_StaticFiles_Associated~",
                table: "PossibleAnswerEntityStaticFileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionEntityStaticFileEntity_AssessmentQuestions_Question~",
                table: "QuestionEntityStaticFileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionEntityStaticFileEntity_StaticFiles_AssociatedStatic~",
                table: "QuestionEntityStaticFileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StaticFiles_AssessmentModuleVersions_AssessmentModuleVersio~",
                table: "StaticFiles");

            migrationBuilder.DeleteData(
                table: "MessageTemplates",
                keyColumn: "Id",
                keyValue: "6863fdeb-ed8d-41ba-8567-c00cf8561470");

            migrationBuilder.DeleteData(
                table: "MessageTemplates",
                keyColumn: "Id",
                keyValue: "a5091d38-fa5e-4cdb-b4bc-22381aeaf8be");

            migrationBuilder.DeleteData(
                table: "MessageTemplates",
                keyColumn: "Id",
                keyValue: "bfc0e145-396f-4bc1-ae2f-14e528fe55b3");

            migrationBuilder.DeleteData(
                table: "MessageTemplates",
                keyColumn: "Id",
                keyValue: "f2a1a4c8-8e6a-4f1c-b9f8-9f2c4c622dd9");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "UserConfigurations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "DataJson",
                table: "UserConfigurations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserConfigurations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Terms",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "Terms",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "NextTermBills",
                table: "Terms",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextTermBegins",
                table: "Terms",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Terms",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Terms",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Terms",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Terms",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SystemSettings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SystemSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                table: "Subjects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Subjects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Subjects",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalScore",
                table: "SubjectScores",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TestScore",
                table: "SubjectScores",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectRemark",
                table: "SubjectScores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "SubjectScores",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentAssessmentId",
                table: "SubjectScores",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "SubjectScores",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExamScore",
                table: "SubjectScores",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SubjectScores",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "TrackGames",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMarksObtained",
                table: "StudentAssessments",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMarksObtainable",
                table: "StudentAssessments",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimesSchoolOpened",
                table: "StudentAssessments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimesPresent",
                table: "StudentAssessments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimesAbsent",
                table: "StudentAssessments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TermId",
                table: "StudentAssessments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Swims",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "StudentAssessments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "SocialActivities",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "StudentAssessments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Regularity",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Punctuality",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishedAt",
                table: "StudentAssessments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PositionInClass",
                table: "StudentAssessments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OverallGrade",
                table: "StudentAssessments",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NumberInClass",
                table: "StudentAssessments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Neatness",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Jumps",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLockedForParents",
                table: "StudentAssessments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "IndoorGames",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HeadTeacherComment",
                table: "StudentAssessments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FieldGames",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerId",
                table: "StudentAssessments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "StudentAssessments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ClassTeacherComment",
                table: "StudentAssessments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClassLevelId",
                table: "StudentAssessments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "AverageScore",
                table: "StudentAssessments",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AttitudeInSchool",
                table: "StudentAssessments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "StudentAssessments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadedAtUtc",
                table: "StaticFiles",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "StaticFiles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsModuleLevelFile",
                table: "StaticFiles",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "StaticFiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssessmentModuleVersionEntityId",
                table: "StaticFiles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssessmentModuleId",
                table: "StaticFiles",
                type: "uuid",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "StaticFiles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sessions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Sessions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Sessions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "PermissionId",
                table: "RolePermissionLinks",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "RolePermissionLinks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "TextResponse",
                table: "QuestionResponses",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid[]>(
                name: "SelectedAnswerIds",
                table: "QuestionResponses",
                type: "uuid[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RespondedAtUtc",
                table: "QuestionResponses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionType",
                table: "QuestionResponses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "QuestionResponses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModuleProgressId",
                table: "QuestionResponses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "QuestionResponses",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "QuestionResponses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionsId",
                table: "QuestionEntityStaticFileEntity",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssociatedStaticFilesId",
                table: "QuestionEntityStaticFileEntity",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "PossibleAnswersId",
                table: "PossibleAnswerEntityStaticFileEntity",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssociatedStaticFilesId",
                table: "PossibleAnswerEntityStaticFileEntity",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Permissions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Permissions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "ParentStudentLinks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Relationship",
                table: "ParentStudentLinks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "ParentStudentLinks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ParentStudentLinks",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Pages",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Pages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Pages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Pages",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "JsonData",
                table: "Pages",
                type: "character varying(20480)",
                maxLength: 20480,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20480);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Pages",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Pages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Pages",
                type: "character varying(20480)",
                maxLength: 20480,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20480);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Pages",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAtUtc",
                table: "ModuleProgress",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionRandomizationSeed",
                table: "ModuleProgress",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasStarted",
                table: "ModuleProgress",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupMemberId",
                table: "ModuleProgress",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerId",
                table: "ModuleProgress",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamTakerAssignmentId",
                table: "ModuleProgress",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "DurationInMinutes",
                table: "ModuleProgress",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "ModuleProgress",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssessmentModuleVersionId",
                table: "ModuleProgress",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "AnswerRandomizationSeed",
                table: "ModuleProgress",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ModuleProgress",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "MessageTemplates",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MessageTemplates",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "MessageTemplates",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10000);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "MessageTemplates",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LogEntries",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "LogEntries",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "LogEntries",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "RequestId",
                table: "LogEntries",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "LogEntries",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "LogEntries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Exception",
                table: "LogEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "LogEntries",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LogEntries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<bool>(
                name: "WaitModuleCompletion",
                table: "Groups",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "Groups",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Groups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Groups",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "Groups",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedTitle",
                table: "Groups",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMemberOrderLocked",
                table: "Groups",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Groups",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "Groups",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Groups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Groups",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                table: "GroupMembers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "GroupMembers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssessmentModuleId",
                table: "GroupMembers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GroupMembers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GradingSchemas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "GradingSchemas",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GradingSchemas",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "GradeRanges",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "GradeRanges",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "MinScore",
                table: "GradeRanges",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "MaxScore",
                table: "GradeRanges",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "GradingSchemaId",
                table: "GradeRanges",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GradeRanges",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "ExamTakers",
                type: "character varying(254)",
                maxLength: 254,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 254,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "ExamTakers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ExamTakers",
                type: "character varying(254)",
                maxLength: 254,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 254,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "ExamTakers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "ExamTakers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionNumber",
                table: "ExamTakers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ExamTakers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "TabSwitchCount",
                table: "ExamTakerAssignments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastTabSwitchAtUtc",
                table: "ExamTakerAssignments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLocked",
                table: "ExamTakerAssignments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerId",
                table: "ExamTakerAssignments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerDisplayName",
                table: "ExamTakerAssignments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssignmentId",
                table: "ExamTakerAssignments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ExamTakerAssignments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "SectionOrArm",
                table: "ClassLevels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderIndex",
                table: "ClassLevels",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ClassLevels",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "GradingSchemaId",
                table: "ClassLevels",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClassLevels",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectsId",
                table: "ClassLevelSubjects",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClassLevelsId",
                table: "ClassLevelSubjects",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUser",
                table: "Banners",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Banners",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Banners",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Banners",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDateUtc",
                table: "Banners",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowToAuthenticatedUsersOnly",
                table: "Banners",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDismissible",
                table: "Banners",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDateUtc",
                table: "Banners",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "Banners",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Banners",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Banners",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Banners",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUser",
                table: "Assignments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Assignments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Assignments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "Assignments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDateUtc",
                table: "Assignments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowResultsImmediately",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "RandomizeQuestions",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "RandomizeAnswers",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedTitle",
                table: "Assignments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "MaxTabSwitches",
                table: "Assignments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublished",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "Assignments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDateUtc",
                table: "Assignments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Assignments",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "Assignments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Assignments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClassLevelId",
                table: "Assignments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assignments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "AssessmentQuestions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "AssessmentQuestions",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "AssessmentQuestions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssessmentModuleVersionId",
                table: "AssessmentQuestions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AssessmentQuestions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "AssessmentPossibleAnswers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "AssessmentPossibleAnswers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "AssessmentPossibleAnswers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "AssessmentPossibleAnswers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AssessmentPossibleAnswers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "AssessmentModules",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "AssessmentModules",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "AssessmentModules",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AssessmentModules",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "AssessmentModuleVersions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUser",
                table: "AssessmentModuleVersions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "AssessmentModuleVersions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "PassingScorePercentage",
                table: "AssessmentModuleVersions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedTitle",
                table: "AssessmentModuleVersions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublished",
                table: "AssessmentModuleVersions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "DurationInMinutes",
                table: "AssessmentModuleVersions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AssessmentModuleVersions",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "AssessmentModuleVersions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "AssessmentModuleVersions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssessmentModuleId",
                table: "AssessmentModuleVersions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AssessmentModuleVersions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "TwoFactorEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "LockoutEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AspNetUserTokens",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                table: "AspNetUserClaims",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                table: "AspNetUserClaims",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetUserClaims",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedName",
                table: "AspNetRoles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetRoles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetRoles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                table: "AspNetRoleClaims",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                table: "AspNetRoleClaims",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetRoleClaims",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.InsertData(
                table: "MessageTemplates",
                columns: new[] { "Id", "Body", "Name", "Subject" },
                values: new object[,]
                {
                    { new Guid("6863fdeb-ed8d-41ba-8567-c00cf8561470"), "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Welcome to ExamNova</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .cta {\r\n        display:block;\r\n        margin:20px auto 10px auto;\r\n        width:max-content;\r\n        text-decoration:none;\r\n        background:#0b5fff;\r\n        color:#ffffff !important;\r\n        padding:12px 20px;\r\n        border-radius:10px;\r\n        font-weight:600;\r\n      }\r\n      .link-fallback {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:12px 0 0 0;\r\n        font-family:monospace;\r\n        font-size:14px;\r\n        color:#0b5fff;\r\n        word-break:break-all;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .cta { background:#7cb8ff; color:#0f1115 !important; }\r\n        .link-fallback { background:#252b36; color:#7cb8ff; }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Welcome to ExamNova</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Your account on <a href=\"{{BaseUrl}}\" target=\"_blank\" rel=\"noopener\" style=\"color:#0b5fff;font-weight:600;text-decoration:none;\">ExamNova</a> has been created successfully.  \r\n          We’re glad to welcome you to the platform.\r\n        </p>\r\n\r\n        <!-- Primary CTA button -->\r\n        <a class=\"cta\" href=\"{{CreatePasswordUrl}}\" target=\"_blank\" rel=\"noopener\">Create Your Password</a>\r\n\r\n        <!-- Fallback plain link -->\r\n        <div class=\"link-fallback\">\r\n          {{CreatePasswordUrl}}\r\n        </div>\r\n\r\n        <p style=\"margin-top:16px;\">If you didn’t sign up for a ExamNova account, you can safely ignore this email.</p>\r\n        <p>Best regards,<br/>ExamNova Team</p>\r\n\r\n        <div class=\"footer\">\r\n          Your account is ready — let’s get started!\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Welcome Email with Create Password Link", "Dear {{User}}, welcome to ExamNova!" },
                    { new Guid("a5091d38-fa5e-4cdb-b4bc-22381aeaf8be"), "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Reset Your Password</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .cta {\r\n        display:block;\r\n        margin:20px auto 10px auto;\r\n        width:max-content;\r\n        text-decoration:none;\r\n        background:#0b5fff;\r\n        color:#ffffff !important;\r\n        padding:12px 20px;\r\n        border-radius:10px;\r\n        font-weight:600;\r\n      }\r\n      .link-fallback {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:12px 0 0 0;\r\n        font-family:monospace;\r\n        font-size:14px;\r\n        color:#0b5fff;\r\n        word-break:break-all;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .link-fallback {\r\n          background:#252b36;\r\n          color:#7cb8ff;\r\n        }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Reset Your Password</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>We received a request to reset your password. Click the button below to create a new one:</p>\r\n\r\n        <!-- Primary CTA uses the reset link -->\r\n        <a class=\"cta\" href=\"{{ResetLink}}\" target=\"_blank\" rel=\"noopener\">Reset Password</a>\r\n\r\n        <!-- Fallback plain link (placed under {{ResetLink}}) -->\r\n        <div class=\"link-fallback\">\r\n          {{ResetLink}}\r\n        </div>\r\n\r\n        <p style=\"margin-top:16px;\">If you didn’t request a password reset, you can safely ignore this email.</p>\r\n        <p>Best regards,<br/>ExamNova Team</p>\r\n\r\n        <div class=\"footer\">\r\n          This link will expire after a short period for security reasons.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Forget Password", "Dear {{User}}, here is your password reset link" },
                    { new Guid("bfc0e145-396f-4bc1-ae2f-14e528fe55b3"), "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Password Reset</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .password-box {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:16px 0;\r\n        font-family:monospace;\r\n        font-size:16px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .password-box {\r\n          background:#252b36;\r\n          color:#7cb8ff;\r\n        }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Password Reset by Administrator</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Your password has been reset by the <strong>system administrator</strong>.  \r\n        </p>\r\n        <p>Here is your new password:</p>\r\n        <div class=\"password-box\">\r\n          {{Password}}\r\n        </div>\r\n        <p>Best regards,<br/>ExamNova Team</p>\r\n        <div class=\"footer\">\r\n          If you did not expect this reset, please contact support immediately.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Password Reset Email", "Dear {{User}}, your password has been reset" },
                    { new Guid("f2a1a4c8-8e6a-4f1c-b9f8-9f2c4c622dd9"), "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Welcome to ExamNova</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Welcome to ExamNova</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Thank you for joining <span style=\"color:#0b5fff;font-weight:600;\">ExamNova</span>!  \r\n          We are excited to have you on board.\r\n        </p>\r\n        <p>Best regards,<br/>ExamNova Team</p>\r\n        <div class=\"footer\">\r\n          If you did not create a ExamNova account, you can safely ignore this message.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Welcome Email", "Dear {{User}}, welcome to ExamNova!" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentModuleVersions_AssessmentModules_AssessmentModule~",
                table: "AssessmentModuleVersions",
                column: "AssessmentModuleId",
                principalTable: "AssessmentModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentQuestions_AssessmentModuleVersions_AssessmentModu~",
                table: "AssessmentQuestions",
                column: "AssessmentModuleVersionId",
                principalTable: "AssessmentModuleVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleProgress_AssessmentModuleVersions_AssessmentModuleVer~",
                table: "ModuleProgress",
                column: "AssessmentModuleVersionId",
                principalTable: "AssessmentModuleVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PossibleAnswerEntityStaticFileEntity_AssessmentPossibleAnsw~",
                table: "PossibleAnswerEntityStaticFileEntity",
                column: "PossibleAnswersId",
                principalTable: "AssessmentPossibleAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PossibleAnswerEntityStaticFileEntity_StaticFiles_Associated~",
                table: "PossibleAnswerEntityStaticFileEntity",
                column: "AssociatedStaticFilesId",
                principalTable: "StaticFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionEntityStaticFileEntity_AssessmentQuestions_Question~",
                table: "QuestionEntityStaticFileEntity",
                column: "QuestionsId",
                principalTable: "AssessmentQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionEntityStaticFileEntity_StaticFiles_AssociatedStatic~",
                table: "QuestionEntityStaticFileEntity",
                column: "AssociatedStaticFilesId",
                principalTable: "StaticFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaticFiles_AssessmentModuleVersions_AssessmentModuleVersio~",
                table: "StaticFiles",
                column: "AssessmentModuleVersionEntityId",
                principalTable: "AssessmentModuleVersions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentModuleVersions_AssessmentModules_AssessmentModule~",
                table: "AssessmentModuleVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentQuestions_AssessmentModuleVersions_AssessmentModu~",
                table: "AssessmentQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleProgress_AssessmentModuleVersions_AssessmentModuleVer~",
                table: "ModuleProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_PossibleAnswerEntityStaticFileEntity_AssessmentPossibleAnsw~",
                table: "PossibleAnswerEntityStaticFileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_PossibleAnswerEntityStaticFileEntity_StaticFiles_Associated~",
                table: "PossibleAnswerEntityStaticFileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionEntityStaticFileEntity_AssessmentQuestions_Question~",
                table: "QuestionEntityStaticFileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionEntityStaticFileEntity_StaticFiles_AssociatedStatic~",
                table: "QuestionEntityStaticFileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StaticFiles_AssessmentModuleVersions_AssessmentModuleVersio~",
                table: "StaticFiles");

            migrationBuilder.DeleteData(
                table: "MessageTemplates",
                keyColumn: "Id",
                keyValue: new Guid("6863fdeb-ed8d-41ba-8567-c00cf8561470"));

            migrationBuilder.DeleteData(
                table: "MessageTemplates",
                keyColumn: "Id",
                keyValue: new Guid("a5091d38-fa5e-4cdb-b4bc-22381aeaf8be"));

            migrationBuilder.DeleteData(
                table: "MessageTemplates",
                keyColumn: "Id",
                keyValue: new Guid("bfc0e145-396f-4bc1-ae2f-14e528fe55b3"));

            migrationBuilder.DeleteData(
                table: "MessageTemplates",
                keyColumn: "Id",
                keyValue: new Guid("f2a1a4c8-8e6a-4f1c-b9f8-9f2c4c622dd9"));

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "UserConfigurations",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "DataJson",
                table: "UserConfigurations",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserConfigurations",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "StartDate",
                table: "Terms",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "Terms",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "NextTermBills",
                table: "Terms",
                type: "TEXT",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NextTermBegins",
                table: "Terms",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Terms",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "IsActive",
                table: "Terms",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "EndDate",
                table: "Terms",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Terms",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SystemSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SystemSettings",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "TEXT",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                table: "Subjects",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Subjects",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Subjects",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "TotalScore",
                table: "SubjectScores",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TestScore",
                table: "SubjectScores",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectRemark",
                table: "SubjectScores",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "SubjectScores",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "StudentAssessmentId",
                table: "SubjectScores",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "SubjectScores",
                type: "TEXT",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExamScore",
                table: "SubjectScores",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "SubjectScores",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "TrackGames",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TotalMarksObtained",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TotalMarksObtainable",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimesSchoolOpened",
                table: "StudentAssessments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimesPresent",
                table: "StudentAssessments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimesAbsent",
                table: "StudentAssessments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TermId",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Swims",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "StudentAssessments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "SocialActivities",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Regularity",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Punctuality",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PublishedAt",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PositionInClass",
                table: "StudentAssessments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OverallGrade",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NumberInClass",
                table: "StudentAssessments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Neatness",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Jumps",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsLockedForParents",
                table: "StudentAssessments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "IndoorGames",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HeadTeacherComment",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FieldGames",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerId",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAt",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "ClassTeacherComment",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassLevelId",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "AverageScore",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AttitudeInSchool",
                table: "StudentAssessments",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StudentAssessments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedAtUtc",
                table: "StaticFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "StaticFiles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsModuleLevelFile",
                table: "StaticFiles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "StaticFiles",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "AssessmentModuleVersionEntityId",
                table: "StaticFiles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssessmentModuleId",
                table: "StaticFiles",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StaticFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "StartDate",
                table: "Sessions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sessions",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "IsActive",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "EndDate",
                table: "Sessions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Sessions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "PermissionId",
                table: "RolePermissionLinks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "RolePermissionLinks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TextResponse",
                table: "QuestionResponses",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SelectedAnswerIds",
                table: "QuestionResponses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid[]),
                oldType: "uuid[]");

            migrationBuilder.AlterColumn<string>(
                name: "RespondedAtUtc",
                table: "QuestionResponses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionType",
                table: "QuestionResponses",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionId",
                table: "QuestionResponses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ModuleProgressId",
                table: "QuestionResponses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "IsCorrect",
                table: "QuestionResponses",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuestionResponses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionsId",
                table: "QuestionEntityStaticFileEntity",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "AssociatedStaticFilesId",
                table: "QuestionEntityStaticFileEntity",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "PossibleAnswersId",
                table: "PossibleAnswerEntityStaticFileEntity",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "AssociatedStaticFilesId",
                table: "PossibleAnswerEntityStaticFileEntity",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Permissions",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Permissions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "ParentStudentLinks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Relationship",
                table: "ParentStudentLinks",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "ParentStudentLinks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ParentStudentLinks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Pages",
                type: "TEXT",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedAtUtc",
                table: "Pages",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Pages",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Pages",
                type: "TEXT",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "JsonData",
                table: "Pages",
                type: "TEXT",
                maxLength: 20480,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20480)",
                oldMaxLength: 20480);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Pages",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAtUtc",
                table: "Pages",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Pages",
                type: "TEXT",
                maxLength: 20480,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20480)",
                oldMaxLength: 20480);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Pages",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "StartedAtUtc",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRandomizationSeed",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HasStarted",
                table: "ModuleProgress",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "GroupMemberId",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerId",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerAssignmentId",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "DurationInMinutes",
                table: "ModuleProgress",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "CompletedAtUtc",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssessmentModuleVersionId",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "AnswerRandomizationSeed",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ModuleProgress",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "MessageTemplates",
                type: "TEXT",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MessageTemplates",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "MessageTemplates",
                type: "TEXT",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "MessageTemplates",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LogEntries",
                type: "TEXT",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "LogEntries",
                type: "TEXT",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Timestamp",
                table: "LogEntries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "RequestId",
                table: "LogEntries",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "LogEntries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "LogEntries",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Exception",
                table: "LogEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "LogEntries",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LogEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "WaitModuleCompletion",
                table: "Groups",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "Groups",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedAtUtc",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Groups",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Groups",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedTitle",
                table: "Groups",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "IsMemberOrderLocked",
                table: "Groups",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Groups",
                type: "TEXT",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "Groups",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAtUtc",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                table: "GroupMembers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "GroupMembers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "AssessmentModuleId",
                table: "GroupMembers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "GroupMembers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GradingSchemas",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "IsActive",
                table: "GradingSchemas",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "GradingSchemas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "GradeRanges",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "GradeRanges",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "MinScore",
                table: "GradeRanges",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "MaxScore",
                table: "GradeRanges",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "GradingSchemaId",
                table: "GradeRanges",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "GradeRanges",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "ExamTakers",
                type: "TEXT",
                maxLength: 254,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(254)",
                oldMaxLength: 254,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "ExamTakers",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ExamTakers",
                type: "TEXT",
                maxLength: 254,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(254)",
                oldMaxLength: 254,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DateOfBirth",
                table: "ExamTakers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAtUtc",
                table: "ExamTakers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionNumber",
                table: "ExamTakers",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ExamTakers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "TabSwitchCount",
                table: "ExamTakerAssignments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "LastTabSwitchAtUtc",
                table: "ExamTakerAssignments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsLocked",
                table: "ExamTakerAssignments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerId",
                table: "ExamTakerAssignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ExamTakerDisplayName",
                table: "ExamTakerAssignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AssignmentId",
                table: "ExamTakerAssignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ExamTakerAssignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "SectionOrArm",
                table: "ClassLevels",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderIndex",
                table: "ClassLevels",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ClassLevels",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "GradingSchemaId",
                table: "ClassLevels",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ClassLevels",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectsId",
                table: "ClassLevelSubjects",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ClassLevelsId",
                table: "ClassLevelSubjects",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUser",
                table: "Banners",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedAtUtc",
                table: "Banners",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Banners",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Banners",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "StartDateUtc",
                table: "Banners",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "ShowToAuthenticatedUsersOnly",
                table: "Banners",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "IsDismissible",
                table: "Banners",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "EndDateUtc",
                table: "Banners",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "Banners",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAtUtc",
                table: "Banners",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Banners",
                type: "TEXT",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Banners",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUser",
                table: "Assignments",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedAtUtc",
                table: "Assignments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Assignments",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Assignments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StartDateUtc",
                table: "Assignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "ShowResultsImmediately",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "RandomizeQuestions",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "RandomizeAnswers",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedTitle",
                table: "Assignments",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "MaxTabSwitches",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "IsPublished",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "Assignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "EndDateUtc",
                table: "Assignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Assignments",
                type: "TEXT",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "Assignments",
                type: "TEXT",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAtUtc",
                table: "Assignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "ClassLevelId",
                table: "Assignments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Assignments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "AssessmentQuestions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "AssessmentQuestions",
                type: "TEXT",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "AssessmentQuestions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "AssessmentModuleVersionId",
                table: "AssessmentQuestions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AssessmentQuestions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "AssessmentPossibleAnswers",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionId",
                table: "AssessmentPossibleAnswers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "AssessmentPossibleAnswers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "IsCorrect",
                table: "AssessmentPossibleAnswers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AssessmentPossibleAnswers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "AssessmentModules",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "AssessmentModules",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAtUtc",
                table: "AssessmentModules",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AssessmentModules",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "AssessmentModuleVersions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUser",
                table: "AssessmentModuleVersions",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "AssessmentModuleVersions",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "PassingScorePercentage",
                table: "AssessmentModuleVersions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedTitle",
                table: "AssessmentModuleVersions",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "IsPublished",
                table: "AssessmentModuleVersions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "DurationInMinutes",
                table: "AssessmentModuleVersions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AssessmentModuleVersions",
                type: "TEXT",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUser",
                table: "AssessmentModuleVersions",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAtUtc",
                table: "AssessmentModuleVersions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "AssessmentModuleId",
                table: "AssessmentModuleVersions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AssessmentModuleVersions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TwoFactorEnabled",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LockoutEnabled",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAtUtc",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionNumber",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                table: "AspNetUserClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetUserClaims",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedName",
                table: "AspNetRoles",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "TEXT",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetRoles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetRoles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                table: "AspNetRoleClaims",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetRoleClaims",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.InsertData(
                table: "MessageTemplates",
                columns: new[] { "Id", "Body", "Name", "Subject" },
                values: new object[,]
                {
                    { "6863fdeb-ed8d-41ba-8567-c00cf8561470", "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Welcome to ExamNova</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .cta {\r\n        display:block;\r\n        margin:20px auto 10px auto;\r\n        width:max-content;\r\n        text-decoration:none;\r\n        background:#0b5fff;\r\n        color:#ffffff !important;\r\n        padding:12px 20px;\r\n        border-radius:10px;\r\n        font-weight:600;\r\n      }\r\n      .link-fallback {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:12px 0 0 0;\r\n        font-family:monospace;\r\n        font-size:14px;\r\n        color:#0b5fff;\r\n        word-break:break-all;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .cta { background:#7cb8ff; color:#0f1115 !important; }\r\n        .link-fallback { background:#252b36; color:#7cb8ff; }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Welcome to ExamNova</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Your account on <a href=\"{{BaseUrl}}\" target=\"_blank\" rel=\"noopener\" style=\"color:#0b5fff;font-weight:600;text-decoration:none;\">ExamNova</a> has been created successfully.  \r\n          We’re glad to welcome you to the platform.\r\n        </p>\r\n\r\n        <!-- Primary CTA button -->\r\n        <a class=\"cta\" href=\"{{CreatePasswordUrl}}\" target=\"_blank\" rel=\"noopener\">Create Your Password</a>\r\n\r\n        <!-- Fallback plain link -->\r\n        <div class=\"link-fallback\">\r\n          {{CreatePasswordUrl}}\r\n        </div>\r\n\r\n        <p style=\"margin-top:16px;\">If you didn’t sign up for a ExamNova account, you can safely ignore this email.</p>\r\n        <p>Best regards,<br/>ExamNova Team</p>\r\n\r\n        <div class=\"footer\">\r\n          Your account is ready — let’s get started!\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Welcome Email with Create Password Link", "Dear {{User}}, welcome to ExamNova!" },
                    { "a5091d38-fa5e-4cdb-b4bc-22381aeaf8be", "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Reset Your Password</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .cta {\r\n        display:block;\r\n        margin:20px auto 10px auto;\r\n        width:max-content;\r\n        text-decoration:none;\r\n        background:#0b5fff;\r\n        color:#ffffff !important;\r\n        padding:12px 20px;\r\n        border-radius:10px;\r\n        font-weight:600;\r\n      }\r\n      .link-fallback {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:12px 0 0 0;\r\n        font-family:monospace;\r\n        font-size:14px;\r\n        color:#0b5fff;\r\n        word-break:break-all;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .link-fallback {\r\n          background:#252b36;\r\n          color:#7cb8ff;\r\n        }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Reset Your Password</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>We received a request to reset your password. Click the button below to create a new one:</p>\r\n\r\n        <!-- Primary CTA uses the reset link -->\r\n        <a class=\"cta\" href=\"{{ResetLink}}\" target=\"_blank\" rel=\"noopener\">Reset Password</a>\r\n\r\n        <!-- Fallback plain link (placed under {{ResetLink}}) -->\r\n        <div class=\"link-fallback\">\r\n          {{ResetLink}}\r\n        </div>\r\n\r\n        <p style=\"margin-top:16px;\">If you didn’t request a password reset, you can safely ignore this email.</p>\r\n        <p>Best regards,<br/>ExamNova Team</p>\r\n\r\n        <div class=\"footer\">\r\n          This link will expire after a short period for security reasons.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Forget Password", "Dear {{User}}, here is your password reset link" },
                    { "bfc0e145-396f-4bc1-ae2f-14e528fe55b3", "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Password Reset</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .password-box {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:16px 0;\r\n        font-family:monospace;\r\n        font-size:16px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .password-box {\r\n          background:#252b36;\r\n          color:#7cb8ff;\r\n        }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Password Reset by Administrator</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Your password has been reset by the <strong>system administrator</strong>.  \r\n        </p>\r\n        <p>Here is your new password:</p>\r\n        <div class=\"password-box\">\r\n          {{Password}}\r\n        </div>\r\n        <p>Best regards,<br/>ExamNova Team</p>\r\n        <div class=\"footer\">\r\n          If you did not expect this reset, please contact support immediately.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Password Reset Email", "Dear {{User}}, your password has been reset" },
                    { "f2a1a4c8-8e6a-4f1c-b9f8-9f2c4c622dd9", "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Welcome to ExamNova</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Welcome to ExamNova</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Thank you for joining <span style=\"color:#0b5fff;font-weight:600;\">ExamNova</span>!  \r\n          We are excited to have you on board.\r\n        </p>\r\n        <p>Best regards,<br/>ExamNova Team</p>\r\n        <div class=\"footer\">\r\n          If you did not create a ExamNova account, you can safely ignore this message.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Welcome Email", "Dear {{User}}, welcome to ExamNova!" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentModuleVersions_AssessmentModules_AssessmentModule~",
                table: "AssessmentModuleVersions",
                column: "AssessmentModuleId",
                principalTable: "AssessmentModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentQuestions_AssessmentModuleVersions_AssessmentModu~",
                table: "AssessmentQuestions",
                column: "AssessmentModuleVersionId",
                principalTable: "AssessmentModuleVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleProgress_AssessmentModuleVersions_AssessmentModuleVer~",
                table: "ModuleProgress",
                column: "AssessmentModuleVersionId",
                principalTable: "AssessmentModuleVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PossibleAnswerEntityStaticFileEntity_AssessmentPossibleAnsw~",
                table: "PossibleAnswerEntityStaticFileEntity",
                column: "PossibleAnswersId",
                principalTable: "AssessmentPossibleAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PossibleAnswerEntityStaticFileEntity_StaticFiles_Associated~",
                table: "PossibleAnswerEntityStaticFileEntity",
                column: "AssociatedStaticFilesId",
                principalTable: "StaticFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionEntityStaticFileEntity_AssessmentQuestions_Question~",
                table: "QuestionEntityStaticFileEntity",
                column: "QuestionsId",
                principalTable: "AssessmentQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionEntityStaticFileEntity_StaticFiles_AssociatedStatic~",
                table: "QuestionEntityStaticFileEntity",
                column: "AssociatedStaticFilesId",
                principalTable: "StaticFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaticFiles_AssessmentModuleVersions_AssessmentModuleVersio~",
                table: "StaticFiles",
                column: "AssessmentModuleVersionEntityId",
                principalTable: "AssessmentModuleVersions",
                principalColumn: "Id");
        }
    }
}
