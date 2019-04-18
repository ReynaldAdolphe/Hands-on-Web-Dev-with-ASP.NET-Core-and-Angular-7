using Microsoft.EntityFrameworkCore;

namespace DVDMovie.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts)
            : base(opts) { }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Studio> Studios { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().HasMany<Rating>(p => p.Ratings)
            .WithOne(r => r.Movie).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Movie>().HasOne<Studio>(p => p.Studio)
            .WithMany(s => s.Movies).OnDelete(DeleteBehavior.SetNull);
        }
    }
}