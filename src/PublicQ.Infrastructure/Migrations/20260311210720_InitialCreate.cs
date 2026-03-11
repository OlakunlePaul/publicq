using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentModules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    ShowToAuthenticatedUsersOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsDismissible = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedByUser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamTakers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTakers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    WaitModuleCompletion = table.Column<bool>(type: "boolean", nullable: false),
                    IsMemberOrderLocked = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    UserEmail = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    RequestId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Body = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Body = table.Column<string>(type: "character varying(20480)", maxLength: 20480, nullable: false),
                    JsonData = table.Column<string>(type: "character varying(20480)", maxLength: 20480, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "UserConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DataJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentModuleVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    PassingScorePercentage = table.Column<int>(type: "integer", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedByUser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssessmentModuleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentModuleVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentModuleVersions_AssessmentModules_AssessmentModule~",
                        column: x => x.AssessmentModuleId,
                        principalTable: "AssessmentModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    StartDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShowResultsImmediately = table.Column<bool>(type: "boolean", nullable: false),
                    RandomizeQuestions = table.Column<bool>(type: "boolean", nullable: false),
                    RandomizeAnswers = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUser = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UpdatedByUser = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<int>(type: "integer", nullable: false),
                    AssessmentModuleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_AssessmentModules_AssessmentModuleId",
                        column: x => x.AssessmentModuleId,
                        principalTable: "AssessmentModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AssessmentModuleVersionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentQuestions_AssessmentModuleVersions_AssessmentModu~",
                        column: x => x.AssessmentModuleVersionId,
                        principalTable: "AssessmentModuleVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaticFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AssessmentModuleId = table.Column<Guid>(type: "uuid", maxLength: 200, nullable: false),
                    Label = table.Column<string>(type: "text", nullable: true),
                    UploadedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsModuleLevelFile = table.Column<bool>(type: "boolean", nullable: false),
                    AssessmentModuleVersionEntityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaticFiles_AssessmentModuleVersions_AssessmentModuleVersio~",
                        column: x => x.AssessmentModuleVersionEntityId,
                        principalTable: "AssessmentModuleVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaticFiles_AssessmentModules_AssessmentModuleId",
                        column: x => x.AssessmentModuleId,
                        principalTable: "AssessmentModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamTakerAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamTakerId = table.Column<string>(type: "text", nullable: false),
                    ExamTakerDisplayName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTakerAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTakerAssignments_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentPossibleAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentPossibleAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentPossibleAnswers_AssessmentQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "AssessmentQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionEntityStaticFileEntity",
                columns: table => new
                {
                    AssociatedStaticFilesId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionEntityStaticFileEntity", x => new { x.AssociatedStaticFilesId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_QuestionEntityStaticFileEntity_AssessmentQuestions_Question~",
                        column: x => x.QuestionsId,
                        principalTable: "AssessmentQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionEntityStaticFileEntity_StaticFiles_AssociatedStatic~",
                        column: x => x.AssociatedStaticFilesId,
                        principalTable: "StaticFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionRandomizationSeed = table.Column<Guid>(type: "uuid", nullable: true),
                    AnswerRandomizationSeed = table.Column<Guid>(type: "uuid", nullable: true),
                    HasStarted = table.Column<bool>(type: "boolean", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "integer", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExamTakerAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamTakerId = table.Column<string>(type: "text", nullable: false),
                    GroupMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssessmentModuleVersionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleProgress_AssessmentModuleVersions_AssessmentModuleVer~",
                        column: x => x.AssessmentModuleVersionId,
                        principalTable: "AssessmentModuleVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleProgress_ExamTakerAssignments_ExamTakerAssignmentId",
                        column: x => x.ExamTakerAssignmentId,
                        principalTable: "ExamTakerAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleProgress_GroupMembers_GroupMemberId",
                        column: x => x.GroupMemberId,
                        principalTable: "GroupMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PossibleAnswerEntityStaticFileEntity",
                columns: table => new
                {
                    AssociatedStaticFilesId = table.Column<Guid>(type: "uuid", nullable: false),
                    PossibleAnswersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PossibleAnswerEntityStaticFileEntity", x => new { x.AssociatedStaticFilesId, x.PossibleAnswersId });
                    table.ForeignKey(
                        name: "FK_PossibleAnswerEntityStaticFileEntity_AssessmentPossibleAnsw~",
                        column: x => x.PossibleAnswersId,
                        principalTable: "AssessmentPossibleAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PossibleAnswerEntityStaticFileEntity_StaticFiles_Associated~",
                        column: x => x.AssociatedStaticFilesId,
                        principalTable: "StaticFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SelectedAnswerIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    TextResponse = table.Column<string>(type: "text", nullable: true),
                    QuestionType = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    RespondedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModuleProgressId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionResponses_AssessmentQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "AssessmentQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionResponses_ModuleProgress_ModuleProgressId",
                        column: x => x.ModuleProgressId,
                        principalTable: "ModuleProgress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MessageTemplates",
                columns: new[] { "Id", "Body", "Name", "Subject" },
                values: new object[,]
                {
                    { new Guid("6863fdeb-ed8d-41ba-8567-c00cf8561470"), "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Welcome to PublicQ</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .cta {\r\n        display:block;\r\n        margin:20px auto 10px auto;\r\n        width:max-content;\r\n        text-decoration:none;\r\n        background:#0b5fff;\r\n        color:#ffffff !important;\r\n        padding:12px 20px;\r\n        border-radius:10px;\r\n        font-weight:600;\r\n      }\r\n      .link-fallback {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:12px 0 0 0;\r\n        font-family:monospace;\r\n        font-size:14px;\r\n        color:#0b5fff;\r\n        word-break:break-all;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .cta { background:#7cb8ff; color:#0f1115 !important; }\r\n        .link-fallback { background:#252b36; color:#7cb8ff; }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Welcome to PublicQ</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Your account on <a href=\"{{BaseUrl}}\" target=\"_blank\" rel=\"noopener\" style=\"color:#0b5fff;font-weight:600;text-decoration:none;\">PublicQ</a> has been created successfully.  \r\n          We’re glad to welcome you to the platform.\r\n        </p>\r\n\r\n        <!-- Primary CTA button -->\r\n        <a class=\"cta\" href=\"{{CreatePasswordUrl}}\" target=\"_blank\" rel=\"noopener\">Create Your Password</a>\r\n\r\n        <!-- Fallback plain link -->\r\n        <div class=\"link-fallback\">\r\n          {{CreatePasswordUrl}}\r\n        </div>\r\n\r\n        <p style=\"margin-top:16px;\">If you didn’t sign up for a PublicQ account, you can safely ignore this email.</p>\r\n        <p>Best regards,<br/>PublicQ Team</p>\r\n\r\n        <div class=\"footer\">\r\n          Your account is ready — let’s get started!\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Welcome Email with Create Password Link", "Dear {{User}}, welcome to PublicQ!" },
                    { new Guid("a5091d38-fa5e-4cdb-b4bc-22381aeaf8be"), "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Reset Your Password</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .cta {\r\n        display:block;\r\n        margin:20px auto 10px auto;\r\n        width:max-content;\r\n        text-decoration:none;\r\n        background:#0b5fff;\r\n        color:#ffffff !important;\r\n        padding:12px 20px;\r\n        border-radius:10px;\r\n        font-weight:600;\r\n      }\r\n      .link-fallback {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:12px 0 0 0;\r\n        font-family:monospace;\r\n        font-size:14px;\r\n        color:#0b5fff;\r\n        word-break:break-all;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .link-fallback {\r\n          background:#252b36;\r\n          color:#7cb8ff;\r\n        }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Reset Your Password</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>We received a request to reset your password. Click the button below to create a new one:</p>\r\n\r\n        <!-- Primary CTA uses the reset link -->\r\n        <a class=\"cta\" href=\"{{ResetLink}}\" target=\"_blank\" rel=\"noopener\">Reset Password</a>\r\n\r\n        <!-- Fallback plain link (placed under {{ResetLink}}) -->\r\n        <div class=\"link-fallback\">\r\n          {{ResetLink}}\r\n        </div>\r\n\r\n        <p style=\"margin-top:16px;\">If you didn’t request a password reset, you can safely ignore this email.</p>\r\n        <p>Best regards,<br/>PublicQ Team</p>\r\n\r\n        <div class=\"footer\">\r\n          This link will expire after a short period for security reasons.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Forget Password", "Dear {{User}}, here is your password reset link" },
                    { new Guid("bfc0e145-396f-4bc1-ae2f-14e528fe55b3"), "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Password Reset</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .password-box {\r\n        background:#f1f4fb;\r\n        border-radius:8px;\r\n        padding:12px 16px;\r\n        margin:16px 0;\r\n        font-family:monospace;\r\n        font-size:16px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .password-box {\r\n          background:#252b36;\r\n          color:#7cb8ff;\r\n        }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Password Reset by Administrator</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Your password has been reset by the <strong>system administrator</strong>.  \r\n        </p>\r\n        <p>Here is your new password:</p>\r\n        <div class=\"password-box\">\r\n          {{Password}}\r\n        </div>\r\n        <p>Best regards,<br/>PublicQ Team</p>\r\n        <div class=\"footer\">\r\n          If you did not expect this reset, please contact support immediately.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Password Reset Email", "Dear {{User}}, your password has been reset" },
                    { new Guid("f2a1a4c8-8e6a-4f1c-b9f8-9f2c4c622dd9"), "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"margin:0;padding:0;\">\r\n  <head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\" />\r\n    <meta name=\"color-scheme\" content=\"light dark\" />\r\n    <meta name=\"supported-color-schemes\" content=\"light dark\" />\r\n    <title>Welcome to PublicQ</title>\r\n    <style>\r\n      body {\r\n        margin:0;\r\n        padding:0;\r\n        background:linear-gradient(135deg,#e6f0ff,#f9fbff);\r\n        font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Ubuntu,'Helvetica Neue',Arial,sans-serif;\r\n        color:#1e2330;\r\n      }\r\n      .container {\r\n        max-width:800px;\r\n        margin:0 auto;\r\n        padding:32px 20px;\r\n      }\r\n      .card {\r\n        background:rgba(255,255,255,0.85);\r\n        border-radius:16px;\r\n        padding:32px;\r\n        box-shadow:0 8px 20px rgba(0,0,0,0.1);\r\n      }\r\n      h1 {\r\n        margin:0 0 20px 0;\r\n        font-size:24px;\r\n        font-weight:600;\r\n        color:#0b5fff;\r\n        text-align:center;\r\n      }\r\n      p {\r\n        margin:0 0 16px 0;\r\n        font-size:16px;\r\n        line-height:1.6;\r\n      }\r\n      .footer {\r\n        margin-top:28px;\r\n        font-size:12px;\r\n        color:#6b7280;\r\n        text-align:center;\r\n      }\r\n      @media (max-width:600px) {\r\n        .card { padding:20px; }\r\n        h1 { font-size:20px; }\r\n        p { font-size:15px; }\r\n      }\r\n      @media (prefers-color-scheme: dark) {\r\n        body {\r\n          background:linear-gradient(135deg,#0f1115,#1b1e24);\r\n          color:#e7e7ea;\r\n        }\r\n        .card {\r\n          background:rgba(30,33,40,0.85);\r\n          border:1px solid #2a2f3a;\r\n        }\r\n        h1 { color:#7cb8ff; }\r\n        .footer { color:#a7acb8; }\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div class=\"container\">\r\n      <div class=\"card\">\r\n        <h1>Welcome to PublicQ</h1>\r\n        <p>Hello <strong>{{User}}</strong>,</p>\r\n        <p>\r\n          Thank you for joining <span style=\"color:#0b5fff;font-weight:600;\">PublicQ</span>!  \r\n          We are excited to have you on board.\r\n        </p>\r\n        <p>Best regards,<br/>PublicQ Team</p>\r\n        <div class=\"footer\">\r\n          If you did not create a PublicQ account, you can safely ignore this message.\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>", "Default Welcome Email", "Dear {{User}}, welcome to PublicQ!" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Name", "Value" },
                values: new object[,]
                {
                    { "AssessmentServiceOptions:MaxPageSize", "100" },
                    { "AssignmentServiceOptions:MaxPageSize", "100" },
                    { "AuthOptions:JwtSettings:Audience", "PublicQClient" },
                    { "AuthOptions:JwtSettings:Issuer", "PublicQServer" },
                    { "AuthOptions:JwtSettings:Secret", "ChangeMe:VGhpc0lzQVNlY3VyZURlZmF1bHRKV1RTZWNyZXRLZXlGb3JEZXZlbG9wbWVudFB1cnBvc2VzT25seQ==" },
                    { "AuthOptions:JwtSettings:TokenExpiryMinutes", "60" },
                    { "DbLoggerOptions:Enabled", "true" },
                    { "DbLoggerOptions:ExcludedCategories:0", "PublicQ.Infrastructure.DynamicJwtBearerHandler" },
                    { "DbLoggerOptions:ExcludedCategories:1", "Microsoft" },
                    { "DbLoggerOptions:ExcludedCategories:2", "System" },
                    { "DbLoggerOptions:LogLevel", "Error" },
                    { "DbLoggerOptions:MaxPageSize", "100" },
                    { "DbLoggerOptions:RetentionPeriodInDays", "90" },
                    { "EmailOptions:MessageProvider", "Sendgrid" },
                    { "EmailOptions:SendFrom", "change-me@publiq.local" },
                    { "FileStorageOptions:MaxUploadFileSizeInKilobytes", "5120" },
                    { "FileStorageOptions:StaticContentPath", "static" },
                    { "GroupServiceOptions:MaxPageSize", "100" },
                    { "InitialSetupOptions:IsInitialized", "false" },
                    { "IpRateLimitOptions:GeneralRules:0:Endpoint", "*" },
                    { "IpRateLimitOptions:GeneralRules:0:Limit", "100" },
                    { "IpRateLimitOptions:GeneralRules:0:Period", "10s" },
                    { "IpRateLimitOptions:IpWhitelist:0", "127.0.0.1" },
                    { "IpRateLimitOptions:IpWhitelist:1", "::1" },
                    { "IpRateLimitOptions:RealIpHeader", "X-Forwarded-For" },
                    { "LlmIntegrationOptions:Enabled", "false" },
                    { "LlmIntegrationOptions:Provider", "OpenAI" },
                    { "PasswordPolicyOptions:RequireDigit", "true" },
                    { "PasswordPolicyOptions:RequireLowercase", "true" },
                    { "PasswordPolicyOptions:RequireNonAlphanumeric", "false" },
                    { "PasswordPolicyOptions:RequireUppercase", "true" },
                    { "PasswordPolicyOptions:RequiredLength", "8" },
                    { "RedisOptions:AbortOnConnectFail", "false" },
                    { "RedisOptions:ConnectionString", "redis:6379" },
                    { "RedisOptions:DefaultDurationInMinutes", "60" },
                    { "RedisOptions:Enabled", "false" },
                    { "RedisOptions:KeyPrefix", "PublicQ:" },
                    { "RedisOptions:ServiceDurationsInMinutes:IReportingService", "60" },
                    { "ReportingServiceOptions:MaxPageSize", "100" },
                    { "SendgridOptions:ApiKey", "<Your Sendgrid API Key>" },
                    { "SmtpOptions:SmtpHost", "localhost" },
                    { "SmtpOptions:SmtpPort", "25" },
                    { "SmtpOptions:UseSsl", "false" },
                    { "SmtpOptions:UseStartTls", "false" },
                    { "UserServiceOptions:MaxImportSize", "500" },
                    { "UserServiceOptions:MaxPageSize", "100" },
                    { "UserServiceOptions:SelfServiceRegistrationEnabled", "true" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentModuleVersions_AssessmentModuleId",
                table: "AssessmentModuleVersions",
                column: "AssessmentModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentModuleVersions_NormalizedTitle",
                table: "AssessmentModuleVersions",
                column: "NormalizedTitle");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentModuleVersions_Title",
                table: "AssessmentModuleVersions",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentPossibleAnswers_QuestionId",
                table: "AssessmentPossibleAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQuestions_AssessmentModuleVersionId",
                table: "AssessmentQuestions",
                column: "AssessmentModuleVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_GroupId",
                table: "Assignments",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_Title",
                table: "Assignments",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banners_ShowToAuthenticatedUsersOnly",
                table: "Banners",
                column: "ShowToAuthenticatedUsersOnly");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTakerAssignments_AssignmentId",
                table: "ExamTakerAssignments",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTakers_NormalizedEmail",
                table: "ExamTakers",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_AssessmentModuleId",
                table: "GroupMembers",
                column: "AssessmentModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId_OrderNumber",
                table: "GroupMembers",
                columns: new[] { "GroupId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_NormalizedTitle",
                table: "Groups",
                column: "NormalizedTitle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Level",
                table: "LogEntries",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Timestamp",
                table: "LogEntries",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_AssessmentModuleVersionId",
                table: "ModuleProgress",
                column: "AssessmentModuleVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_ExamTakerAssignmentId",
                table: "ModuleProgress",
                column: "ExamTakerAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_GroupMemberId",
                table: "ModuleProgress",
                column: "GroupMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_PossibleAnswerEntityStaticFileEntity_PossibleAnswersId",
                table: "PossibleAnswerEntityStaticFileEntity",
                column: "PossibleAnswersId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionEntityStaticFileEntity_QuestionsId",
                table: "QuestionEntityStaticFileEntity",
                column: "QuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_ModuleProgressId",
                table: "QuestionResponses",
                column: "ModuleProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_QuestionId",
                table: "QuestionResponses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticFiles_AssessmentModuleId",
                table: "StaticFiles",
                column: "AssessmentModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticFiles_AssessmentModuleVersionEntityId",
                table: "StaticFiles",
                column: "AssessmentModuleVersionEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "ExamTakers");

            migrationBuilder.DropTable(
                name: "LogEntries");

            migrationBuilder.DropTable(
                name: "MessageTemplates");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "PossibleAnswerEntityStaticFileEntity");

            migrationBuilder.DropTable(
                name: "QuestionEntityStaticFileEntity");

            migrationBuilder.DropTable(
                name: "QuestionResponses");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "UserConfigurations");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AssessmentPossibleAnswers");

            migrationBuilder.DropTable(
                name: "StaticFiles");

            migrationBuilder.DropTable(
                name: "ModuleProgress");

            migrationBuilder.DropTable(
                name: "AssessmentQuestions");

            migrationBuilder.DropTable(
                name: "ExamTakerAssignments");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "AssessmentModuleVersions");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "AssessmentModules");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
