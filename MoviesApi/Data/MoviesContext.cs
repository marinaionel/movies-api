using Microsoft.EntityFrameworkCore;
using MoviesApi.Core.Model;
using System.Collections.Generic;

namespace MoviesApi.Data
{
    public class MoviesContext : DbContext
    {
        public MoviesContext()
        {
        }

        public MoviesContext(DbContextOptions<MoviesContext> options) : base(options)
        {
        }

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.HasDefaultSchema("moviesfile");

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("people_pk")
                      .IsClustered(false);

                entity.ToTable("people", "moviesfile");

                entity.HasIndex(e => e.Id, "people_id_uindex")
                      .IsUnique();

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("id");

                entity.Property(e => e.Birth)
                      .HasColumnName("birth");

                entity.Property(e => e.Name)
                      .HasColumnType("text")
                      .HasColumnName("name");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("movies_pk")
                      .IsClustered(false);

                entity.ToTable("movies", "moviesfile");

                entity.HasIndex(e => e.Id, "movies_id_uindex")
                      .IsUnique();

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("id");

                entity.Property(e => e.Title)
                      .HasColumnType("text")
                      .HasColumnName("title");

                entity.Property(e => e.Year)
                      .HasColumnName("year");

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
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ratings", "moviesfile");

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
        }
    }
}