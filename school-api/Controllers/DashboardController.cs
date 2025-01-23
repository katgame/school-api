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
using school_api.Data.Models;

namespace school_api.Controllers
{
  //  [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Player)]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        
        [HttpGet("get-dashboard")]
        public IActionResult GetDashboard()
        {
            var response = _dashboardService.GetDashboard();
            return Ok(response);
        }

        [HttpPut("update-school")]
        public IActionResult UpdateSchool([FromBody] School request)
        {
            var response = _dashboardService.UpdateSchool(request);
            return Ok(response);
        }

        [HttpPost("create-school")]
        public IActionResult CreateSchool([FromBody] School request)
        {
            var response = _dashboardService.CreateSchool(request);
            return Ok(response);
        }

    }
}
