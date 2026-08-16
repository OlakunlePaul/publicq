using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCumulativeTermScoresTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Terms\" ALTER COLUMN \"IsCumulativeTerm\" DROP DEFAULT;");
            migrationBuilder.Sql("ALTER TABLE \"Terms\" ALTER COLUMN \"IsCumulativeTerm\" TYPE boolean USING CASE WHEN \"IsCumulativeTerm\" = 0 THEN false ELSE true END;");
            migrationBuilder.Sql("ALTER TABLE \"Terms\" ALTER COLUMN \"IsCumulativeTerm\" SET DEFAULT false;");
            migrationBuilder.Sql("ALTER TABLE \"SubjectScores\" ALTER COLUMN \"CumulativeAverage\" TYPE numeric USING \"CumulativeAverage\"::numeric;");
            migrationBuilder.Sql("ALTER TABLE \"SubjectScores\" ALTER COLUMN \"FirstTermScore\" TYPE numeric USING \"FirstTermScore\"::numeric;");
            migrationBuilder.Sql("ALTER TABLE \"SubjectScores\" ALTER COLUMN \"SecondTermScore\" TYPE numeric USING \"SecondTermScore\"::numeric;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
