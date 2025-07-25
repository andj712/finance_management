using finance_management.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace finance_management.Commands.SplitTransactions
{
    public class SplitTransactionCommand : IRequest<Unit>
    {
        [Required]
        public string TransactionId { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one split is required")]
        public List<SingleCategorySplit> Splits { get; set; } 
       
    }
}
