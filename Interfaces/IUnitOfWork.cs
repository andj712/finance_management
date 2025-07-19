namespace finance_management.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
