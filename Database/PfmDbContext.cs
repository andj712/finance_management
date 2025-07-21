using finance_management.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_management.Database
{
    public class PfmDbContext:DbContext
    {
        public PfmDbContext(DbContextOptions<PfmDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(15);
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.BeneficiaryName).HasMaxLength(30);
                entity.Property(e => e.Description).HasMaxLength(30);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Direction)
                  .HasConversion<string>();
                entity.Property(e => e.Kind)
                    .HasConversion<string>();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
                var port = Environment.GetEnvironmentVariable("DATABASE_PORT");
                var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");
                var user = Environment.GetEnvironmentVariable("DATABASE_USER");
                var password = Environment.GetEnvironmentVariable("DATABASE_PASS");

                var connectionString = $"Host={host};Port={port};Database={dbName};Username={user};Password={password}";
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

    }
}
