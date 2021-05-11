using Microsoft.EntityFrameworkCore;
using MoviesApi.Core.Models;

namespace MoviesApi.Data
{
    public class DbContextClass : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Chart> Charts { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbContextClass(DbContextOptions<DbContextClass> contextOptions) : base(contextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
