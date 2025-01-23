using System.Collections.Generic;
using school_api.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace school_api.Data.Dto
{
    public class UserRequest
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}
