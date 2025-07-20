using finance_management.Models;
using MediatR;

namespace finance_management.Queries.GetTransactions
{
    public class GetTransactionsQuery : IRequest<List<Transaction>>
    {
        public string? TransactionKind { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
    }
}
