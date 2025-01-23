using school_api.Data.Dto;
using school_api.Data.Models;
using school_api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
//App TAX on withdrawl


namespace school_api.Data.Services
{
    
    public class AccountService : IAccountService
    {

        const int appTax = 10;
        private AppDbContext _context;
        public AccountService(AppDbContext context)
        {
            _context = context;

        }

        public double CreditFunds(Guid? userID, double creditAmount , string merchantResponse)
        {
            try
            {
                var accountDetails = _context.Transaction.Where(x => x.UserId == userID).ToList();
                var balance = accountDetails.Sum(x => x.Credit - x.Debit);
                var newCredit = new Transaction
                {
                    AccountBalance = balance+= creditAmount,
                    Credit = creditAmount,
                    Debit = 0.0,
                    UserId = userID.Value,
                    AdminTax = false,
                    Id = Guid.NewGuid(),
                    MerchantResponse = merchantResponse,
                    TransactionDate = System.DateTime.Now,
                };
                var newBalance = balance + creditAmount;
                _context.Transaction.Add(newCredit);
                _context.SaveChanges();
                return balance;
            }
            catch (Exception)
            {

                return 0.0;
            }
        }


        public AccountDetails GetAccountBalance(Guid userID)
        {
            var data = _context.Transaction.Where(x => x.UserId == userID).Sum(x => x.Credit - x.Debit);
            return new AccountDetails { AccountBalance = data };
        }
           
        internal double GetCommission(double balance) =>  balance / appTax;

        public bool DebitFunds(Guid userID, double DebitAmount)
        {
            try
            {

                var accountDetails = _context.Transaction.Where(x => x.UserId == userID).ToList();
                var balance = accountDetails.Sum(x => x.Credit - x.Debit);

                if (DebitAmount > balance) return false;

                var commission = GetCommission(DebitAmount);

                var newDebit = new Transaction {
                    AccountBalance = balance - DebitAmount,
                    Credit = 0.0,
                    Debit = DebitAmount,
                    AdminTax = false,
                    UserId = userID,
                    Id = Guid.NewGuid(),
                    TransactionDate = System.DateTime.Now,
                };

                double newBalance = balance - DebitAmount;


                var commisionDebit = new Transaction
                {
                    AccountBalance = newBalance - commission,
                    Credit = 0.0,
                    Debit = commission,
                    AdminTax = true,
                    UserId = userID,
                    Id = Guid.NewGuid(),
                    TransactionDate = System.DateTime.Now,
                };

                var admin = new AdmistrationsService(_context);
                admin.CreditFunds(userID, commission, null);

                _context.Transaction.Add(newDebit);
                _context.Transaction.Add(commisionDebit);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        List<Transaction> IAccountService.GetAccountInformation(Guid userID)
        {
            return _context.Transaction.Where(x => x.UserId == userID).ToList();
        }

        public bool UpdatePlayerAccount(List<UpdateUserAccountRequest> request)
        {
            try
            {
                foreach (var player in request)
                {
                    var balance = _context.Transaction.Where(x => x.UserId == player.UserId).Sum(y => y.Credit - y.Debit);
                    var transaction = new Transaction();
                    double newBalance = 0;
                    switch (player.GameSessionResult)
                    {
                        case Enums.PlayerGameSessionResult.Win:

                            transaction.AccountBalance = balance + player.Price;
                            newBalance = transaction.AccountBalance;
                            transaction.Credit = player.Price;
                            transaction.Debit = 0.0;
                            transaction.UserId = player.UserId;
                            transaction.AdminTax = false;
                            transaction.Id = Guid.NewGuid();
                            transaction.TransactionDate = System.DateTime.Now;
                            break;
                        case Enums.PlayerGameSessionResult.Lose:
                            transaction.AccountBalance = balance - player.Price;
                            newBalance = transaction.AccountBalance;
                            transaction.Credit = 0.0;
                            transaction.Debit = player.Price;
                            transaction.UserId = player.UserId;
                            transaction.AdminTax = false;
                            transaction.Id = Guid.NewGuid();
                            transaction.TransactionDate = System.DateTime.Now;
                            break;
                        case Enums.PlayerGameSessionResult.Cancelled:
                            // Make no changes to user account
                            break;
                    }

                    _context.Transaction.Add(transaction);
                    _context.SaveChanges();
     
                }
                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
