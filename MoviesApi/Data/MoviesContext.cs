using Microsoft.EntityFrameworkCore;
using MoviesApi.Core.Model;

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

        public virtual DbSet<Director> Directors { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Star> Stars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.Entity<Director>(entity =>
            {
                entity.HasKey(e => new { e.MovieId, e.PersonId })
                    .HasName("directors_pk")
                    .IsClustered(false);

                entity.ToTable("directors", "moviesfile");

                entity.Property(e => e.MovieId).HasColumnName("movie_id");

                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.Directors)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("directors_movies_id_fk");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Directors)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("directors_people_id_fk");
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

                entity.Property(e => e.Year).HasColumnName("year");
            });

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

                entity.Property(e => e.Birth).HasColumnName("birth");

                entity.Property(e => e.Name)
                    .HasColumnType("text")
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ratings", "moviesfile");

                entity.Property(e => e.MovieId).HasColumnName("movie_id");

                entity.Property(e => e.AverageRating).HasColumnName("rating");

                entity.Property(e => e.Votes).HasColumnName("votes");

                entity.HasOne(d => d.Movie)
                    .WithMany()
                    .HasForeignKey(d => d.MovieId)
                    .HasConstraintName("ratings_movies_id_fk");
            });

            modelBuilder.Entity<Star>(entity =>
            {
                entity.HasKey(e => new { e.MovieId, e.PersonId })
                    .HasName("stars_pk")
                    .IsClustered(false);

                entity.ToTable("stars", "moviesfile");

                entity.Property(e => e.MovieId).HasColumnName("movie_id");

                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.Stars)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stars_movies_id_fk");
            });
        }
    }
}