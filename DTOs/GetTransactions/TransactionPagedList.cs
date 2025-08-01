using finance_management.Models.Enums;
using Newtonsoft.Json;

namespace finance_management.DTOs.GetTransactions
{
    public class TransactionPagedList
    {

        public int PageSize { get; set; }

        public int Page { get; set; }

        public int TotalCount { get; set; }

        
        public int TotalPages { get; set; }

       
        public SortOrderEnum SortOrder { get; set; }

        
        public string SortBy { get; set; }

        
        public List<TransactionWithSplits> Items { get; set; } = new();
    }
}
