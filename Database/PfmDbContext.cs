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
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.BeneficiaryName).HasMaxLength(200);
                entity.Property(e => e.Direction).HasMaxLength(1);
                entity.Property(e => e.Amount);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.Kind).HasMaxLength(50);

                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.Direction);
            });
        }
    }
}
