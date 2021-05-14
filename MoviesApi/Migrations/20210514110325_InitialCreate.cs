using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "moviesfile");

            migrationBuilder.CreateTable(
                name: "directors",
                schema: "moviesfile",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "int", nullable: true),
                    person_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "movies",
                schema: "moviesfile",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    year = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "people",
                schema: "moviesfile",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    birth = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                schema: "moviesfile",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "int", nullable: true),
                    rating = table.Column<float>(type: "real", nullable: true),
                    votes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "stars",
                schema: "moviesfile",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "int", nullable: true),
                    person_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "directors",
                schema: "moviesfile");

            migrationBuilder.DropTable(
                name: "movies",
                schema: "moviesfile");

            migrationBuilder.DropTable(
                name: "people",
                schema: "moviesfile");

            migrationBuilder.DropTable(
                name: "ratings",
                schema: "moviesfile");

            migrationBuilder.DropTable(
                name: "stars",
                schema: "moviesfile");
        }
    }
}
