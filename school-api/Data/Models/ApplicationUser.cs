using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace school_api.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string GameId { get; set; }
        public string GameState { get; set; }
        public string SchoolId { get; set; }
        public string userUniqueId { get; set; }

        public string GameType { get; set; }
    }
}
