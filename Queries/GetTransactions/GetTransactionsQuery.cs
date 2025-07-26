using finance_management.DTOs.GetTransactions;
using finance_management.Models;
using finance_management.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace finance_management.Queries.GetTransactions
{
    public class GetTransactionsQuery : IRequest<TransactionPagedList>
    {
        [FromQuery(Name = "transaction-kind")]
        public string? TransactionKind { get; set; }
        [FromQuery(Name = "start-date")] 
        public DateTime? StartDate { get; set; }
        [FromQuery(Name = "end-date")] 
        public DateTime? EndDate { get; set; }
        [FromQuery(Name = "page")] 
        public int Page { get; set; } = 1;
        [FromQuery(Name = "page-size")] 
        public int PageSize { get; set; } = 10;
        [FromQuery(Name = "sort-by")] 
        public string? SortBy { get; set; }
        [FromQuery(Name = "sort-order")] 
        public SortOrderEnum SortOrder { get; set; } = SortOrderEnum.Asc;
    }
}
