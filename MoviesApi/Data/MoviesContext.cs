using Microsoft.EntityFrameworkCore;
using MoviesApi.Core.Models;
using System.Collections.Generic;

namespace MoviesApi.Data
{
    public class MoviesContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<TotalRatings> TotalRatings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Chart> Charts { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Country> Countries { get; set; }

        private const string Schema = "moviesfile";

        public MoviesContext()
        {
        }

        public MoviesContext(DbContextOptions<MoviesContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("accounts", Schema);

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id")
                      .HasColumnType("varchar(128)");

                entity.Property(e => e.Birthday)
                      .HasColumnName("birthday")
                      .HasColumnType("date");

                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasColumnType("nvarchar(80)");

                entity.Property(e => e.Email)
                      .HasColumnName("email")
                      .HasColumnType("nvarchar(100)");

                entity.HasMany(m => m.Watchlist)
                    .WithMany(p => p.Watchers)
                    .UsingEntity<Dictionary<string, object>>(
                        "watchlist",
                        a => a
                            .HasOne<Movie>()
                            .WithMany()
                            .HasForeignKey("movie_id")
                            .HasConstraintName("watchlist_movies_id_fk")
                            .OnDelete(DeleteBehavior.Cascade),
                        m => m
                            .HasOne<Account>()
                            .WithMany()
                            .HasForeignKey("account_id")
                            .HasConstraintName("watchlist_accounts_id_fk")
                            .OnDelete(DeleteBehavior.ClientCascade));

                entity.HasMany(m => m.FavouritePeople)
                    .WithMany(p => p.Fans)
                    .UsingEntity<Dictionary<string, object>>(
                        "fans",
                        a => a
                            .HasOne<Person>()
                            .WithMany()
                            .HasForeignKey("person_id")
                            .HasConstraintName("fans_people_id_fk")
                            .OnDelete(DeleteBehavior.Cascade),
                        m => m
                            .HasOne<Account>()
                            .WithMany()
                            .HasForeignKey("account_id")
                            .HasConstraintName("fans_accounts_id_fk")
                            .OnDelete(DeleteBehavior.ClientCascade));
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.Text)
                      .HasColumnName("text");

                entity.Property(e => e.Title)
                      .HasColumnType("title");

                entity.Property(e => e.Rating)
                      .HasColumnName("rating");

                entity.HasKey(e => new { e.MovieId, e.AccountId })
                      .HasName("reviews_pk");

                entity.HasOne(e => e.Account)
                      .WithMany()
                      .HasForeignKey(e => e.AccountId)
                      .HasConstraintName("reviews_accounts_id_fk");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasIndex(g => g.Id)
                      .IsUnique();

                entity.Property(g => g.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(l => l.Name)
                      .HasColumnName("name")
                      .HasColumnType("nvarchar(30)");

                entity.ToTable("country", Schema);
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasIndex(g => g.Id)
                      .IsUnique();

                entity.Property(g => g.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(l => l.Name)
                      .HasColumnName("name")
                      .HasColumnType("nvarchar(30)");

                entity.ToTable("languages", Schema);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasIndex(g => g.Id)
                      .IsUnique();

                entity.Property(g => g.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(g => g.Name)
                      .HasColumnName("name")
                      .HasColumnType("nvarchar(30)");

                entity.ToTable("genres", Schema);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("people_pk")
                      .IsClustered(false);

                entity.ToTable("people", Schema);

                entity.HasIndex(e => e.Id, "people_id_uindex")
                      .IsUnique();

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("id");

                entity.Property(m => m.Description)
                      .HasColumnName("description");

                entity.Property(e => e.Birth)
                      .HasColumnName("birth");

                entity.Property(e => e.Name)
                      .HasColumnType("nvarchar(max)")
                      .HasColumnName("name");

                entity.Property(e => e.ImageUrl)
                      .HasColumnName("image_url")
                      .HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("movies_pk")
                      .IsClustered(false);

                entity.Property(m => m.PosterUrl)
                      .HasColumnType("text")
                      .HasColumnName("poster_url");

                entity.Property(m => m.TrailerYoutubeVideoId)
                      .HasColumnType("text")
                      .HasColumnName("trailer_youtube_video_id");

                entity.Property(m => m.Runtime)
                      .HasColumnType("nvarchar(15)")
                      .HasColumnName("runtime");

                entity.Property(m => m.Plot)
                      .HasColumnType("text")
                      .HasColumnName("plot");

                entity.Property(m => m.BoxOffice)
                      .HasColumnType("nvarchar(30)")
                      .HasColumnName("box_office");

                entity.Property(m => m.ReleaseDate)
                      .HasColumnType("date")
                      .HasColumnName("release_date");

                entity.ToTable("movies", Schema);

                entity.HasIndex(e => e.Id, "movies_id_uindex")
                      .IsUnique();

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("id");

                entity.Property(e => e.Title)
                      .HasColumnType("nvarchar(max)")
                      .HasColumnName("title");

                entity.Property(e => e.Year)
                      .HasColumnName("year");

                entity.HasMany(m => m.Reviews)
                      .WithOne(r => r.Movie)
                      .HasForeignKey(e => e.MovieId)
                      .HasConstraintName("reviews_movies_id_fk");

                entity.HasMany(m => m.Actors)
                      .WithMany(p => p.ActedInMovies)
                      .UsingEntity<Dictionary<string, object>>(
                        "stars",
                        a => a
                            .HasOne<Person>()
                            .WithMany()
                            .HasForeignKey("person_id")
                            .HasConstraintName("stars_people_id_fk")
                            .OnDelete(DeleteBehavior.Cascade),
                        m => m
                            .HasOne<Movie>()
                            .WithMany()
                            .HasForeignKey("movie_id")
                            .HasConstraintName("stars_movies_id_fk")
                            .OnDelete(DeleteBehavior.ClientCascade));

                entity.HasMany(m => m.Directors)
                      .WithMany(p => p.DirectedMovies)
                      .UsingEntity<Dictionary<string, object>>(
                        "directors",
                        a => a
                            .HasOne<Person>()
                            .WithMany()
                            .HasForeignKey("person_id")
                            .HasConstraintName("directors_people_id_fk")
                            .OnDelete(DeleteBehavior.Cascade),
                        m => m
                            .HasOne<Movie>()
                            .WithMany()
                            .HasForeignKey("movie_id")
                            .HasConstraintName("directors_movies_id_fk")
                            .OnDelete(DeleteBehavior.ClientCascade));

                entity.HasMany(m => m.Languages)
                      .WithMany(l => l.Movies)
                      .UsingEntity<Dictionary<string, object>>(
                        "language_movie",
                        a => a
                            .HasOne<Language>()
                            .WithMany()
                            .HasForeignKey("language_id")
                            .HasConstraintName("language_movie_languages_id_fk")
                            .OnDelete(DeleteBehavior.Cascade),
                        m => m
                            .HasOne<Movie>()
                            .WithMany()
                            .HasForeignKey("movie_id")
                            .HasConstraintName("language_movie_movies_id_fk")
                            .OnDelete(DeleteBehavior.ClientCascade));

                entity.HasMany(m => m.Genres)
                      .WithMany(l => l.Movies)
                      .UsingEntity<Dictionary<string, object>>(
                        "genre_movie",
                        a => a
                            .HasOne<Genre>()
                            .WithMany()
                            .HasForeignKey("genre_id")
                            .HasConstraintName("genre_movie_genres_id_fk")
                            .OnDelete(DeleteBehavior.Cascade),
                        m => m
                            .HasOne<Movie>()
                            .WithMany()
                            .HasForeignKey("movie_id")
                            .HasConstraintName("genre_movie_movies_id_fk")
                            .OnDelete(DeleteBehavior.ClientCascade));

                entity.HasMany(m => m.Countries)
                      .WithMany(l => l.Movies)
                      .UsingEntity<Dictionary<string, object>>(
                        "country_movie",
                        a => a
                            .HasOne<Country>()
                            .WithMany()
                            .HasForeignKey("country_id")
                            .HasConstraintName("country_movie_country_id_fk")
                            .OnDelete(DeleteBehavior.Cascade),
                        m => m
                            .HasOne<Movie>()
                            .WithMany()
                            .HasForeignKey("movie_id")
                            .HasConstraintName("country_movie_movies_id_fk")
                            .OnDelete(DeleteBehavior.ClientCascade));
            });

            modelBuilder.Entity<TotalRatings>(entity =>
            {
                entity.HasKey(e => e.MovieId);

                entity.ToTable("ratings", Schema);

                entity.Property(e => e.MovieId)
                      .HasColumnName("movie_id");

                entity.Property(e => e.AverageRating)
                      .HasColumnName("rating");

                entity.Property(e => e.Votes)
                      .HasColumnName("votes");

                entity.HasOne(d => d.Movie)
                      .WithMany()
                      .HasForeignKey(d => d.MovieId)
                      .HasConstraintName("ratings_movies_id_fk");
            });

            modelBuilder.Entity<Chart>(entity =>
            {
                entity.ToTable("charts", Schema);
                entity.HasIndex(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Route)
                      .HasColumnName("route")
                      .HasColumnType("nvarchar(50)");

                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasColumnType("nvarchar(50)");

                entity.HasMany(e => e.Movies)
                      .WithMany(m => m.Charts)
                      .UsingEntity<Dictionary<string, object>>(
                        "charts_movie",
                        a => a
                            .HasOne<Movie>()
                            .WithMany()
                            .HasForeignKey("movie_id")
                            .HasConstraintName("charts_movie_movies_id_fk")
                            .OnDelete(DeleteBehavior.Cascade),
                        m => m
                            .HasOne<Chart>()
                            .WithMany()
                            .HasForeignKey("chart_id")
                            .HasConstraintName("charts_movie_charts_id_fk")
                            .OnDelete(DeleteBehavior.ClientCascade)
                        );
            });
        }
    }
}