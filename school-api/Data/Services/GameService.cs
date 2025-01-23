
using school_api.Data.Dto;
using school_api.Data.Models;
using school_api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static System.Collections.Specialized.BitVector32;


//App TAX on withdrawl


namespace school_api.Data.Services
{

    public class GameService : IGameService
    {
        const int appTax = 10;
        const int duration = 15;
        private AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;
        private string _connectionString;
        DbContextOptionsBuilder<AppDbContext> _optionsBuilder;
        public GameService(AppDbContext context, IConfiguration configuration, IAccountService accountService)
        {
            _context = context;
            _configuration = configuration;
            _optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            _connectionString = _configuration.GetConnectionString("mySql");
            _accountService = accountService;
        }

        //public RequestSessionResponse RequestSession(SessionRequest request)
        //{
        //    var schoolDetails = _context.School.Where(x => x.Id == request.SchoolId).Include(y => y.Session).ThenInclude(k =>
        //    k.GameSession).SingleOrDefault();
        //    var maxPlayersForSession = schoolDetails.MaxPlayers;
        //    var currentActiveSessions = schoolDetails.Session.Where(c => c.Active).ToList();
        //    var avalaibleSession = new Session();
        //    var gameSession = new GameSession();
        //    var newSession = false;

        //    using (var transaction = _context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            if (currentActiveSessions.Count > 0)
        //            {
        //                avalaibleSession = GetVailableSession(currentActiveSessions, maxPlayersForSession);
        //                if (avalaibleSession == null)
        //                {
        //                    var sessionID = Guid.NewGuid();
        //                    avalaibleSession = CreateNewSession(request.UserId, request.SchoolId, sessionID);
        //                    gameSession = AddPlayerToSession(request.UserId, avalaibleSession);
        //                    newSession = true;
        //                    transaction.Commit();
        //                }
        //                else
        //                {
        //                    gameSession = AddPlayerToSession(request.UserId, avalaibleSession);
        //                    transaction.Commit();
        //                }
        //            }
        //            else
        //            {
        //                var sessionID = Guid.NewGuid();
        //                avalaibleSession = CreateNewSession(request.UserId, request.SchoolId, sessionID);
        //                gameSession = AddPlayerToSession(request.UserId, avalaibleSession);
        //                newSession = true;
        //                transaction.Commit();
        //            }
        //        }
        //        catch 
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
               
        //    }

        //    return new RequestSessionResponse
        //    {
        //        SchoolId = request.SchoolId,
        //        UserId = request.UserId,
        //        isNewSession = newSession,
        //        SessionInfo = avalaibleSession
        //    };
        //}

        internal Session GetVailableSession(List<Session> sessions, int maxPlayers)
        {
            return sessions.FirstOrDefault(x => x.GameSession.Count < maxPlayers);
        }

        internal Session CreateNewSession(Guid userId, Guid schoolId, Guid sessionID)
        {
            var newSession = new Session
            {
                Id = sessionID,
                Active = true,
                GameSession = null,
                SchoolId = schoolId,
                SessionStartTime = DateTime.Now
            };
            _context.Session.Add(newSession);
            _context.SaveChanges();
            return newSession;
        }

        internal GameSession AddPlayerToSession(Guid userId, Session session)
        {
            var gameSessionId = Guid.NewGuid();
            var avatar = GetAvatar(session.GameSession);
            var balance = _accountService.GetAccountBalance(userId);
            var newGameSession = new GameSession
            {
                Id = gameSessionId,
                PlayerId = userId,
                SessionId = session.Id,
                GameSessionStartTime = DateTime.Now,
                isPlayerActive = Enums.ConnectionStatus.Active,
                LastPingResponse = DateTime.Now,
                AvatarId = avatar.Id,
                Duration = duration,
                FirstName = "Test",
                LastName = "Tester",
                Tokens = balance.AccountBalance,
                Avatar = avatar
            };
            _context.GameSession.Add(newGameSession);
            _context.SaveChanges();
            return newGameSession;

        }

        Avatar GetAvatar(List<GameSession> GameSessions)
        {
            var allAvatars = _context.Avatar.ToList();
            if (GameSessions == null)
            {
                return allAvatars.FirstOrDefault();
            }

            var availableAvator = allAvatars.Where(p => !GameSessions.Any(p2 => p2.AvatarId == p.Id)).OrderBy(x => x.OrderId).FirstOrDefault();
            return availableAvator;
        }
        async Task ManageGameSessionsAsync()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

            while (await timer.WaitForNextTickAsync())
            {
                try
                {
                    using (var context = new AppDbContext(_optionsBuilder.Options))
                    {
                        var data = await context.GameSession.Where(c => c.isPlayerActive == Enums.ConnectionStatus.Active).ToListAsync();
                        var activeSession = await context.Session.Where(c => c.Active).Include(x => x.GameSession).ToListAsync();
                        foreach (var session in activeSession)
                        {
                            if (!session.GameSession.Where(c => c.isPlayerActive == Enums.ConnectionStatus.Active).Any())
                            {
                                //Delete Game sessions and delete session for resourses
                                context.GameSession.RemoveRange(session.GameSession);
                                context.Session.Remove(session);
                                context.SaveChanges();
                            }
                        }
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                DateTime currentTime = DateTime.Now;
                                TimeSpan timeDifference = currentTime - item.LastPingResponse;

                                if (timeDifference.TotalSeconds > 60)
                                {
                                    item.isPlayerActive = Enums.ConnectionStatus.Offline;
                                    context.GameSession.Remove(item);
                                    context.SaveChanges();
                                }
                                else if (timeDifference.TotalSeconds > 30)
                                {
                                    item.isPlayerActive = Enums.ConnectionStatus.Pending;
                                    context.GameSession.Update(item);
                                    context.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("No active players found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }



        public void StartGameManegement()
        {
            _ = ManageGameSessionsAsync();
        }


        public bool Ping(PingRequest request)
        {
            var session = _context.Session.Where(c => c.Id == request.SessionId).Include(x => x.GameSession).SingleOrDefault();
            if (session != null)
            {
                //Update User ping time
                var playerGameSession = session.GameSession.SingleOrDefault(x => x.Id == request.UserId);
                if (playerGameSession != null)
                {
                    playerGameSession.LastPingResponse = DateTime.Now;
                    _context.GameSession.Update(playerGameSession);
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public List<Session> GetAllSessionInformation()
        {
            return _context.Session.Include(x => x.GameSession).ThenInclude(y => y.Avatar).ToList();
        }

        public Session GetSessionInformationId(Guid sessionId)
        {
            return _context.Session.Where(c => c.Id == sessionId).Include(x => x.GameSession).SingleOrDefault();
        }
    }
}
