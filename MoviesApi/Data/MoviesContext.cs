using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MoviesApi.Core.Enums;
using MoviesApi.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoviesApi.Data
{
    public class MoviesContext : DbContext
    {
        private const string Schema = "moviesfile";

        public MoviesContext()
        {
        }

        public MoviesContext(DbContextOptions<MoviesContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Chart> Charts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasIndex(g => g.Id)
                      .IsUnique();

                entity.Property(g => g.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

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
                      .HasColumnType("text")
                      .HasColumnName("name");

                ValueComparer<ICollection<CrewMemberType>> valueComparer = new(
                                                                    (c1, c2) => c1.SequenceEqual(c2),
                                                                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                                                    c => c.ToHashSet());

                entity.Property(p => p.Jobs).HasColumnName("jobs")
                                            .HasConversion(obj => string.Join(',', obj.Select(e => Enum.GetName(e.GetType(), e))),
                                                           obj => obj.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(o => (CrewMemberType)Enum.Parse(typeof(CrewMemberType), o)).ToHashSet() ?? new HashSet<CrewMemberType>())
                                            .Metadata
                                            .SetValueComparer(valueComparer);
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("movies_pk")
                      .IsClustered(false);

                entity.Property(m => m.PosterUrl)
                      .HasColumnType("text")
                      .HasColumnName("poster_url");

                entity.Property(m => m.Runtime)
                      .HasColumnType("nvarchar(15)")
                      .HasColumnName("runtime");

                entity.Property(m => m.Plot)
                      .HasColumnType("text")
                      .HasColumnName("plot");

                entity.ToTable("movies", Schema);

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

                entity.HasMany(m => m.Genres)
                      .WithMany(g => g.Movies)
                      .UsingEntity(t => t.ToTable("genre_movie"));
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasNoKey();

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