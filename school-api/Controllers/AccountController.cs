using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using school_api.Data.Services;
using school_api.Data.ViewModels;
using school_api.Data.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using school_api.Interfaces;
using school_api.Data.Dto;
using System.Dynamic;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;


namespace school_api.Controllers
{
   // [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Player)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAdminstratorService _accountAdninService;
        string encryptionKey = "abcdefghijklmnopqrstuvwx";
        public AccountController(IAccountService accountService, IAdminstratorService accountAdninService)
        {
            _accountService = accountService;
            _accountAdninService = accountAdninService;
        }

        [HttpPost("credit-funds")]
        public IActionResult CreditDebitFunds([FromBody] AccountRequest request)
        {
            try
            {
                var Decrypt = Encryption.Decrypt(request.encryptedData, encryptionKey);
                RootCredit root = JsonConvert.DeserializeObject<RootCredit>(Decrypt);

                var balance = _accountService.CreditFunds(Guid.Parse(root.sensitiveData[0].userId), double.Parse(root.sensitiveData[0].price), root.sensitiveData[0].MerchantResponse);
                var response = new
                {
                    balance = balance,
                    isSuccess = true
                };
                return Ok(response);
            }
            catch (Exception)
            {
                var response = new
                {
                    balance = 0,
                    isSuccess = false
                };
                return BadRequest(response);
            }
          
        }

        [HttpPost("debit-funds")]
        public IActionResult DebitFunds([FromBody] AccountRequest request)
        {
            var Decrypt = Encryption.Decrypt(request.encryptedData, encryptionKey);
            Root root = JsonConvert.DeserializeObject<Root>(Decrypt);
            var response = _accountService.DebitFunds(Guid.Parse(root.SensitiveData.userId), root.SensitiveData.price);
            return Ok(response);
        }

        [HttpGet("get-account/{userId}")]
        public IActionResult GetAccountDetails(Guid userID)
        {
            var data = _accountService.GetAccountInformation(userID);
            dynamic response = new ExpandoObject();
            response.Transactions = data;
            response.Balance = data.Sum(x => x.Credit - x.Debit);
            return Ok(response);
        }


        [HttpGet("get-account-balance/{userId}")]
        public IActionResult GetAccountBalance(string userId)
        {
            var data = _accountService.GetAccountBalance(Guid.Parse( userId));
            return Ok(data);
        }

        [HttpGet("get-admin-account-balance")]
        public IActionResult GetAdminAccountBalance()
        {
            var data = _accountAdninService.GetAccountBalance();
            return Ok(data);
        }

        [HttpGet("get-admin-account-transactions")]
        public IActionResult GetAdminAccountTransactions()
        {
            var data = _accountAdninService.GetAdminTransaction();
            return Ok(data);
        }

        [HttpPost("update-account")]
        public IActionResult UpdatePlayerAccount([FromBody] AccountRequest request)
        {
            var Decrypt = Encryption.Decrypt(request.encryptedData, encryptionKey);
            RootUpdate root = JsonConvert.DeserializeObject<RootUpdate>(Decrypt);
            try
            {
               // if(IsTransactionTimeValid(root.TransactionTime)) {
                    var response = _accountService.UpdatePlayerAccount(root.SensitiveData);
                    return Ok(response);
                //} else
                //{
                //    return BadRequest("This transaction is not valid");
                //}
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        private bool IsTransactionTimeValid(DateTime dateTime)
        {
            DateTime serverTime = DateTime.UtcNow;

            // Calculate the time difference
            TimeSpan timeDifference = serverTime - dateTime;

            // Check if the absolute difference is within 30 seconds
            return Math.Abs(timeDifference.TotalSeconds) <= 30;
        }
    }
}
