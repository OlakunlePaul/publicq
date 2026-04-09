using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressionLockAndTargetAudience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnforceProgressionLock",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsForEntireClassLevel",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgressionOrder",
                table: "Assignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnforceProgressionLock",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "IsForEntireClassLevel",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "ProgressionOrder",
                table: "Assignments");
        }
    }
}
