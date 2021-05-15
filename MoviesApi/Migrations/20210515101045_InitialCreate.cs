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
                name: "movies",
                schema: "moviesfile",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    year = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("movies_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "people",
                schema: "moviesfile",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    birth = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("people_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
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
                    table.ForeignKey(
                        name: "ratings_movies_id_fk",
                        column: x => x.movie_id,
                        principalSchema: "moviesfile",
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stars",
                schema: "moviesfile",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "int", nullable: false),
                    person_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("stars_pk", x => new { x.movie_id, x.person_id })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "stars_movies_id_fk",
                        column: x => x.movie_id,
                        principalSchema: "moviesfile",
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "directors",
                schema: "moviesfile",
                columns: table => new
                {
                    movie_id = table.Column<int>(type: "int", nullable: false),
                    person_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("directors_pk", x => new { x.movie_id, x.person_id })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "directors_movies_id_fk",
                        column: x => x.movie_id,
                        principalSchema: "moviesfile",
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "directors_people_id_fk",
                        column: x => x.person_id,
                        principalSchema: "moviesfile",
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_directors_person_id",
                schema: "moviesfile",
                table: "directors",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "movies_id_uindex",
                schema: "moviesfile",
                table: "movies",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "people_id_uindex",
                schema: "moviesfile",
                table: "people",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ratings_movie_id",
                schema: "moviesfile",
                table: "ratings",
                column: "movie_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "directors",
                schema: "moviesfile");

            migrationBuilder.DropTable(
                name: "ratings",
                schema: "moviesfile");

            migrationBuilder.DropTable(
                name: "stars",
                schema: "moviesfile");

            migrationBuilder.DropTable(
                name: "people",
                schema: "moviesfile");

            migrationBuilder.DropTable(
                name: "movies",
                schema: "moviesfile");
        }
    }
}
