using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTermAndSessionToExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "Assignments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TermId",
                table: "Assignments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "AssessmentModules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TermId",
                table: "AssessmentModules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_SessionId",
                table: "Assignments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TermId",
                table: "Assignments",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentModules_SessionId",
                table: "AssessmentModules",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentModules_TermId",
                table: "AssessmentModules",
                column: "TermId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentModules_Sessions_SessionId",
                table: "AssessmentModules",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssessmentModules_Terms_TermId",
                table: "AssessmentModules",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Sessions_SessionId",
                table: "Assignments",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Terms_TermId",
                table: "Assignments",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentModules_Sessions_SessionId",
                table: "AssessmentModules");

            migrationBuilder.DropForeignKey(
                name: "FK_AssessmentModules_Terms_TermId",
                table: "AssessmentModules");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Sessions_SessionId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Terms_TermId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_SessionId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TermId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_AssessmentModules_SessionId",
                table: "AssessmentModules");

            migrationBuilder.DropIndex(
                name: "IX_AssessmentModules_TermId",
                table: "AssessmentModules");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "TermId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "AssessmentModules");

            migrationBuilder.DropColumn(
                name: "TermId",
                table: "AssessmentModules");
        }
    }
}
