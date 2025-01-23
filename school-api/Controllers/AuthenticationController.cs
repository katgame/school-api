using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using school_api.Data;
using school_api.Data.Models;
using school_api.Data.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using school_api.Data.Dto;
using school_api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace school_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IProfileService _profileService;
        private readonly IAccountService _accountService;
        private readonly IDashboardService _dashboardService;
        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            IConfiguration configuration, 
            ILogger<AuthenticationController> logger,
            IProfileService profileService,
            IAccountService accountService,
            IDashboardService dashboardService)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _profileService = profileService;
            _accountService = accountService;
            _dashboardService = dashboardService;
        }
        //[Authorize(Roles = UserRoles.Admin)]
        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody]RegisterVM payload)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Please, provide all required fields");
                }

                var userExists = await _userManager.FindByEmailAsync(payload.Email);

                if (userExists != null)
                {
                    return BadRequest($"User {payload.Email} already exists");
                }

                ApplicationUser newUser = new ApplicationUser()
                {
                    Email = payload.Email,
                    UserName = payload.UserName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false
                };


                var result = await _userManager.CreateAsync(newUser, payload.Password);

                if (!result.Succeeded)
                {
                    return BadRequest("User could not be created!");
                }
                await _userManager.SetLockoutEnabledAsync(newUser, false);

                var profileCreated = _profileService.CreateProfile(Guid.Parse(newUser.Id));

                switch (payload.Role)
                {
                    case "Admin":
                        await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                        break;
                    case "Player":
                        await _userManager.AddToRoleAsync(newUser, UserRoles.Player);
                        break;
                  
                
                }

                return Ok("New user successfully created");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("all-user")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var response = users;
            return Ok(response);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("get-user-roles")]
        public async Task<IActionResult> GetUserRoles()
        {
            var roles = await _roleManager.Roles.Where(x => (x.Name == "Admin") || (x.Name == "Player")).ToListAsync();
            return Ok(roles);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("enable-user/{userId}")]
        public async Task<IActionResult> EnableUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.SetLockoutEnabledAsync(user, false);
            return Ok();
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                
                var tokens = _context.RefreshTokens.Where(x => x.UserId == userId).ToList();
                foreach (var token in tokens)
                {
                    _context.RefreshTokens.Remove(token);
                }
                await _userManager.SetLockoutEnabledAsync(user, true);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }
        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody]LoginVM payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, provide all required fields");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            //if(user != null && await _userManager.CheckPasswordAsync(user, payload.Password) && user.LockoutEnabled == false)
            if (user != null && user.LockoutEnabled == false)
                {
                var tokenValue = await GenerateJwtToken(user);
                //_logger.LogTrace("Token value: ", tokenValue);
                //var userRole = await _userManager.GetRolesAsync(user);
                //_logger.LogTrace("userRole value: ", userRole);
                var accountInfo = _accountService.GetAccountBalance(Guid.Parse(user.Id));
                var loginRespose = new LoginRespose
                {
                    token = tokenValue,
                    userDetails = new UserInfo
                    {
                        Id = user.Id,
                        //Role = (List<string>)userRole,
                        Name = user.UserName,
                        Email = user.Email,
                    },
                    userAccount = new AccountDetails
                    {
                        AccountBalance = accountInfo.AccountBalance
                    }
                };
                return Ok(loginRespose);
            }

            return Unauthorized();
        }



        private async Task<AuthResultVM> GenerateJwtToken(ApplicationUser user)
        {
            try
            {
                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                //Add User Roles
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }


                var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.UtcNow.AddHours(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                var refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.UtcNow,
                    DateExpire = DateTime.UtcNow.AddMonths(6),
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
                };
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();

                var response = new AuthResultVM()
                {
                    Token = jwtToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = token.ValidTo
                };

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        [HttpPost("update-game-stats")]
        public async Task<IActionResult> updateGameStats([FromBody] UpdateGameStatsRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request._Id);
                if (user != null)
                {
                    user.GameId = request.GameId;
                    user.GameState = request.GameState;
                    user.userUniqueId = request.UserUniqueId;
                    user.GameType = request.GameType;
                    user.SchoolId = request.SchoolId;
                    _context.Update(user);
                    _context.SaveChanges();
                    return Ok(true);
                } else
                {
                    return Ok(false);
                }
         
            }
            catch (Exception)
            {
                return Ok(false);
            }
      
        }


       // [Authorize]
        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return Ok(user);
        }

        [HttpGet("get-game-info")]
        public async Task<IActionResult> GetGameInformation(string gameId)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.GameId == gameId);
                if(user != null)
                {
                    var schoolPrice = _dashboardService.GetSchoolDetails(Guid.Parse(user.SchoolId)).Price;
                    var gameInfo = new
                    {
                        gameId = user.GameId,
                        gameType = user.GameType,
                        schoolId = user.SchoolId,
                        schoolPrice = schoolPrice
                    };
                    return Ok(gameInfo);
                }
                throw new Exception();
               
            }
            catch(Exception)
            {
                return BadRequest("Game not found");
            }
        }

        [HttpGet("get-school-details")]
        public async Task<IActionResult> GetSchool(string schoolId)
        {
            var users = await _userManager.Users.Where(x => x.SchoolId == schoolId).ToListAsync();
            if(users.Count > 0)
            {
                var schoolData = new
                {
                    numberOfactivePlayers = users.Count()
                };
                return Ok(schoolData);
            } else
            {
                var schoolData = new
                {
                    numberOfactivePlayers = 0
                };

                return Ok(schoolData);
            }
          
        }

        [HttpPost("join-active-room")]
        public async Task<JoinGameResponse> JoinActiveRoom([FromBody] JoinActiveSessionRequest request)
        {
            var joinGameResponse = new JoinGameResponse();
            var activeGames = _userManager.Users.Where(x => x.SchoolId == request.schoolId && x.GameType == request.gameType).ToList();
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (activeGames.Count > 0 & user != null)
            {
                var gameId = activeGames.GroupBy(user => user.GameId).Where(group => group.Count() < 6).Select(group => group.Key).FirstOrDefault();
                if(gameId.IsNullOrEmpty())
                {
                    return new JoinGameResponse
                    {
                        GameId = null,
                    };
                } else
                {
                  
                    //if (user != null)
                    //{
                    //    user.GameId = gameId;
                    //    user.GameState = "TOSTART";
                    //    user.userUniqueId = request.uniqueId;
                    //    user.GameType = request.gameType;
                    //    user.SchoolId = request.schoolId;
                    //    _context.Update(user);
                    //    _context.SaveChanges();
                    //}
                    joinGameResponse.GameId = gameId;
                 
                    joinGameResponse.email = user.Email;
                    return joinGameResponse;
                }
            }
            return new JoinGameResponse
            {
                GameId = null,
            };
        }

        [HttpPost("leave-game")]
        public async Task<IActionResult> LeaveGame(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                user.GameId = null;
                user.GameState = null;
                user.userUniqueId = null;
                user.SchoolId = null;
                user.GameType = null;
                _context.Update(user);
                _context.SaveChanges();
            }
         
            return Ok(true);
        }

        [HttpPost("complete-game")]
        public async Task<IActionResult> CompleteGame(string gameId)
        {
            var users = await _userManager.Users.Where(x => x.GameId == gameId).ToListAsync();
            foreach (var user in users)
            {
                user.GameId = null;
                user.GameState = null;
                user.userUniqueId = null;
                user.SchoolId = null;
                user.GameType = null;
              
            }
            _context.UpdateRange(users);
            _context.SaveChanges();
            return Ok(true);
        }
        [HttpPost("update-game-stats-to-play")]
        public async Task<IActionResult> updateGameStatsToPlay(string gameId)
        {
            var users = await _userManager.Users.Where(x => x.GameId == gameId).ToListAsync();
            users.ForEach(x => x.GameState = "gamePlay");
            _context.UpdateRange(users);
            _context.SaveChanges();
            return Ok(true);
        }
    }

}
