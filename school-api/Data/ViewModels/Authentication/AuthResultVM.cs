﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace school_api.Data.ViewModels.Authentication
{
    public class AuthResultVM
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
