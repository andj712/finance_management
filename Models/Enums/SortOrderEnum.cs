using System.Runtime.Serialization;

namespace finance_management.Models.Enums
{
    public enum SortOrderEnum
    {
        [EnumMember(Value = "asc")]
        Asc,

        [EnumMember(Value = "desc")]
        Desc
    }
}
