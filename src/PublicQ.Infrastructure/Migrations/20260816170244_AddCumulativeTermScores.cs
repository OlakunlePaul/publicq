using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCumulativeTermScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCumulativeTerm",
                table: "Terms",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeAverage",
                table: "SubjectScores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FirstTermScore",
                table: "SubjectScores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SecondTermScore",
                table: "SubjectScores",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCumulativeTerm",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "CumulativeAverage",
                table: "SubjectScores");

            migrationBuilder.DropColumn(
                name: "FirstTermScore",
                table: "SubjectScores");

            migrationBuilder.DropColumn(
                name: "SecondTermScore",
                table: "SubjectScores");
        }
    }
}
