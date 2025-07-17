using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace finance_management.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionKindEnum
    {
        [EnumMember(Value = "dep")]
        Deposit,

        [EnumMember(Value = "wdw")]
        Withdrawal,

        [EnumMember(Value = "pmt")]
        Payment,

        [EnumMember(Value = "fee")]
        Fee,

        [EnumMember(Value = "inc")]
        Interest,

        [EnumMember(Value = "rev")]
        Reversal,

        [EnumMember(Value = "adj")]
        Adjustment,

        [EnumMember(Value = "lnd")]
        LoanDisbursement,

        [EnumMember(Value = "lnr")]
        LoanRepayment,

        [EnumMember(Value = "fcx")]
        ForexExchange,

        [EnumMember(Value = "aop")]
        AccountOpening,

        [EnumMember(Value = "acl")]
        AccountClosing,

        [EnumMember(Value = "spl")]
        Split,

        [EnumMember(Value = "sal")]
        Salary
    }
}
