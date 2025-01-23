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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Dynamic;


namespace school_api.Controllers
{
   // [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Player)]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
           
        }

        //[HttpPost("request-session")]
        //public IActionResult RequestSession([FromBody] SessionRequest request)
        //{
        //    var response = _gameService.RequestSession(request);
        //    return Ok(response);
        //}

        [HttpGet("start-game-management")]
        public void StartGameManagement()
        {
            _gameService.StartGameManegement();
        }

        [HttpPost("ping-event")]
        public IActionResult CheckDiviceConnection([FromBody] PingRequest request)
        {
            try
            {
                var isUpdated = _gameService.Ping(request);
                dynamic response = new ExpandoObject();
                response.isActive = isUpdated;
                response.PingTime = System.DateTime.Now;
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest("Could not load logs from the database");
            }
        }

        [HttpGet("session-information")]
        public IActionResult SessionInformation()
        {
            try
            {
                var data = _gameService.GetAllSessionInformation();
              
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest("Could not load logs from the database");
            }
        }

        [HttpGet("session-information-by-id/{sessionId}")]
        public IActionResult SessionInformationById(Guid sessionId)
        {
            try
            {
                var data = _gameService.GetSessionInformationId(sessionId);

                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest("Could not load logs from the database");
            }
        }
    }
}
