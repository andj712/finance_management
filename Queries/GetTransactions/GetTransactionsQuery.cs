using finance_management.Models;
using finance_management.Models.Enums;
using MediatR;

namespace finance_management.Queries.GetTransactions
{
    public class GetTransactionsQuery : IRequest<List<Transaction>>
    {
        public TransactionKindEnum? TransactionKind { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
    }
}
