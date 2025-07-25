using finance_management.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace finance_management.Commands.SplitTransactions
{
    public class SplitTransactionCommand : IRequest<Unit>
    {
        
        public string TransactionId { get; set; }
        
        public List<SingleCategorySplit> Splits { get; set; } 
       
    }
}
