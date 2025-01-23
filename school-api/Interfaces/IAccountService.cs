using school_api.Data.Models;
using school_api.Data.Dto;
namespace school_api.Interfaces
{
    public interface IAccountService
    {
        List<Transaction> GetAccountInformation(Guid userID);
        AccountDetails GetAccountBalance(Guid userID);
        bool DebitFunds(Guid userID, double price);
        double CreditFunds(Guid? userID, double price, string merchantResponse);
        bool UpdatePlayerAccount(List<UpdateUserAccountRequest> request);
    }
}
