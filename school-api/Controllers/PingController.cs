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
    [Authorize(Roles = UserRoles.Player)]
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public PingController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        //[HttpGet("ping")]
        //public IActionResult CheckDiviceConnection(schoolId, GamesessionId, UserID)
        //{
        //    try
        //    {
        //        return Ok("User still active...");
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest("Could not load logs from the database");
        //    }
        //}
    }
}
