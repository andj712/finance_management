using finance_management.Models;
using finance_management.Models.Enums;
using MediatR;

namespace finance_management.Queries.GetSpendingAnalytics
{
    public class GetSpendingAnalyticsQuery : IRequest<SpendingAnalytics>
    {
        public string? CatCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DirectionEnum? Direction { get; set; }
    }
}
