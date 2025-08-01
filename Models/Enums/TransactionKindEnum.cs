using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace finance_management.Models.Enums
{
    public enum TransactionKindEnum
    {
        dep,        // Deposit  
        wdw,        // Withdrawal  
        pmt,        // Payment  
        fee,        // Fee  
        inc,        // Interest  
        rev,        // Reversal  
        adj,        // Adjustment  
        lnd,        // LoanDisbursement  
        lnr,        // LoanRepayment  
        fcx,        // ForeignExchange  
        aop,        // AccountOpening  
        acl,        // AccountClosing  
        spl,        // SplitPayment  
        sal         // Salary  
    }
}
