using school_api.Data;
using school_api.Data.Models;
using school_api.Interfaces;
using System;

namespace school_api.Data.Services
{
    public class ProfileService : IProfileService
    {
        private AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateProfile(Guid userID)
        {
            try
            {
                var request = new Transaction { AccountBalance = 0.0, Id = Guid.NewGuid(), UserId = userID, AdminTax = false, Credit = 0.0, Debit = 0.0, TransactionDate = DateTime.Now };
                _context.Transaction.Add(request);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

    }
}
