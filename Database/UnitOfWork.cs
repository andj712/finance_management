using finance_management.Interfaces;

namespace finance_management.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PfmDbContext _context;

        public UnitOfWork(PfmDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
