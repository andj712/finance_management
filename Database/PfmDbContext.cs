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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Split> Splits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(15);
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.BeneficiaryName).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(50);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Direction)
                  .HasConversion<string>();
                entity.Property(e => e.Kind)
                    .HasConversion<string>();
                // sekundardni kljuc za CatCode
                entity.HasOne(e => e.Category)
                      .WithMany(e => e.Transactions)
                      .HasForeignKey(e => e.CatCode)
                      .HasPrincipalKey(e => e.Code)
                      .OnDelete(DeleteBehavior.SetNull);

                // indeks za bolje perfomanse
                entity.HasIndex(e => e.CatCode);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Code);
                entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.ParentCode).HasMaxLength(10);

                // Self-referencing relationship
                entity.HasOne(e => e.ParentCategory)
                      .WithMany(e => e.ChildCategories)
                      .HasForeignKey(e => e.ParentCode)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ParentCode);
            });
            modelBuilder.Entity<Split>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.CatCode });
                entity.Property(e => e.Amount).HasPrecision(18, 2);

                entity.HasOne(e => e.Transaction)
                      .WithMany(t => t.Splits)
                      .HasForeignKey(e => e.TransactionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Splits)
                      .HasForeignKey(e => e.CatCode)
                      .HasPrincipalKey(c => c.Code)
                      .OnDelete(DeleteBehavior.Restrict);
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
