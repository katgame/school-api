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

namespace school_api.Controllers
{
    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Player)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("create-profile")]
        public IActionResult CreateProfile(Guid UserId)
        {
            var response = _profileService.CreateProfile(UserId);
            return Ok(response);
        }

       
    }
}
