using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace finance_management.Models
{
    public class Split
    {

        [Key]
        [Column(Order = 0)]
        public string TransactionId { get; set; } = string.Empty;

        [Key]
        [Column(Order = 1)]
        public string CatCode { get; set; } = string.Empty;

        [Required]
        [Precision(18, 2)]
        public double Amount { get; set; }

        // Navigation properties
        [ForeignKey("TransactionId")]
        [JsonIgnore]
        public virtual Transaction Transaction { get; set; }

        [ForeignKey("CatCode")]
        public virtual Category Category { get; set; }
    }
}
