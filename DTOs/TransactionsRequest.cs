    using finance_management.Models.Enums;
    using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace finance_management.DTOs
    {
        public class GetTransactionsQueryDTO 
        {
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public TransactionKindEnum? TransactionKind { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
            public int Page { get; set; } = 1;

            [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
            public int PageSize { get; set; } = 10;

            public string? SortBy { get; set; }

            public string SortOrder { get; set; } = "asc";

           
        }
    }