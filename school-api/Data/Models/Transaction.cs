using Microsoft.EntityFrameworkCore;
using System;

namespace school_api.Data.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public double AccountBalance { get; set; } 
        public double Credit { get; set; }
        public double Debit { get; set; }
        public bool AdminTax { get; set; }
        public DateTime TransactionDate { get; set; }
        public Guid? TransactionSessionId { get; set; } = Guid.Empty;
        public Guid? TransactionGameSessionId { get; set; } = Guid.Empty;
        public string MerchantResponse { get; set; }
    }
}

