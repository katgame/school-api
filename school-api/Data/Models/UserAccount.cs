using System;

namespace school_api.Data.Models
{
    public class UserAccount
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public double Balance { get; set; }
        public double PreviousBalance { get; set; }
        public DateTime BalanceUpdatedDate { get; set; }
        public DateTime PreviousBalanceUpdatedDate { get; set; }

    }
}
