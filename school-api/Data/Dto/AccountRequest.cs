using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;

namespace school_api.Data.Dto
{
    public class SensitiveData
    {
        public string userId { get; set; }
        public int price { get; set; }
        public bool isDebit { get; set; }
        public string MerchantResponse { get; set; }
    }

    public class RootUpdate
    {
        public List<UpdateUserAccountRequest> SensitiveData { get; set; }
        public DateTime TransactionTime { get; set; }
    }

    public class Root
    {
        public SensitiveData SensitiveData { get; set; }
    }

    public class AccountRequest
    {
        public string encryptedData { get; set; }
    }

    public class RootCredit
    {
        public List<SensitiveDatum> sensitiveData { get; set; }
        public string transactionTime { get; set; }
    }

    public class SensitiveDatum
    {
        public string userId { get; set; }
        public string price { get; set; }
        public string gameSessionResult { get; set; }
        public string MerchantResponse { get; set; }
    }

}
