using System.Collections.Generic;
using school_api.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace school_api.Data.Dto
{
    public class UpdateGameStatsRequest
    {
        public string _Id { get; set; }
        public string GameId { get; set; }
        public string GameState { get; set; }
        public string GameType { get; set; }
        public string SchoolId { get; set; }
        public string UserUniqueId { get; set; }
    }
}
