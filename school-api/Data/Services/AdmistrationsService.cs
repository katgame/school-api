using school_api.Data.Dto;
using school_api.Data.Models;
using school_api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace school_api.Data.Services
{
    public class AdmistrationsService : IAdminstratorService
    {
        private AppDbContext _context;
        public AdmistrationsService(AppDbContext context)
        {
            _context = context;
        }

        public double CreditFunds(Guid? userID, double creditAmount, string m)
        {
            try
            {
                var newCredit = new AdminTransaction
                {
                    AccountBalance = 0.0,
                    Credit = creditAmount,
                    Debit = 0.0,
                    UserId = userID.Value,
                    Id = Guid.NewGuid(),
                    TransactionDate = System.DateTime.Now,
                };
                _context.AdminTransaction.Add(newCredit);
                _context.SaveChanges();
                return 0.0;
            }
            catch (Exception)
            {

                return 0.0;
            }
        }


        public Transaction GetAccountInformation(Guid userID)
            => _context.AdminTransaction.SingleOrDefault(x => x.UserId == userID);

        public bool DebitFunds(Guid userID, double DebitAmount)
        {
            //this account should never be debited
            return false;
        }

        List<Transaction> IAccountService.GetAccountInformation(Guid userID)
        {
            throw new NotImplementedException();
        }

        public AccountDetails GetAccountBalance(Guid userID)
        {
            var data = _context.AdminTransaction.Where(x => x.UserId == userID).Sum(x => x.Credit - x.Debit);
            return new AccountDetails { AccountBalance = data };
        }

        public AccountDetails GetAccountBalance()
        {
            var data = _context.AdminTransaction.Sum(x => x.Credit - x.Debit);
            return new AccountDetails { AccountBalance = data };
        }

        public List<AdminTransaction> GetAdminTransaction()
        {
            return _context.AdminTransaction.ToList();
        }

        public bool UpdatePlayerAccount(List<UpdateUserAccountRequest> request)
        {
            throw new NotImplementedException();
        }
    }
}
