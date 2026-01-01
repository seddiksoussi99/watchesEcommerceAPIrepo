using Microsoft.EntityFrameworkCore;
using WatchesEcommerce.Models.Entities;

namespace WatchesEcommerce.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Watch>()
                .HasOne(w => w.Category)
                .WithMany()
                .HasForeignKey(w => w.CategoryId);

            modelBuilder.Entity<Watch>()
                .HasMany(w => w.Colors)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                l => l.HasOne<Color>().WithMany().HasForeignKey("ColorId"),
                r => r.HasOne<Watch>().WithMany().HasForeignKey("WatchId"),
                j =>
                {
                    j.HasKey("ColorId", "WatchId");
                    j.ToTable("WatchesColors");
                });


            modelBuilder.Entity<Category>()
                .ToTable("Categories");
            modelBuilder.Entity<Color>()
                .ToTable("Colors");
            modelBuilder.Entity<Watch>()
                .ToTable("Watches");
            modelBuilder.Entity<Image>()
                .ToTable("Images");

        }

        public DbSet<Watch> Watches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Image> Images { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<CommandeDetail> CommandeDetails { get; set; }
    }
}
