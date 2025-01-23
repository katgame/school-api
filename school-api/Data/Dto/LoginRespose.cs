using school_api.Data.Models;
using school_api.Data.ViewModels.Authentication;

namespace school_api.Data.Dto
{
    public class LoginRespose
    {
        public AuthResultVM token { get; set; }
        public UserInfo userDetails { get; set; }

        public AccountDetails userAccount { get; set; }
    }
}
