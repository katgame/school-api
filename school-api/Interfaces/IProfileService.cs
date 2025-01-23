using school_api.Data.Models;
using school_api.Data.Dto;
using System.Collections.Generic;
using System;

namespace school_api.Interfaces
{
    public interface IProfileService
    {
        bool CreateProfile(Guid userID);
     
    }
}
