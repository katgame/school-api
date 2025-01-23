using System;
using System.Collections.Generic;

namespace school_api.Data.Dto
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Role { get; set; }
       
    }
}
