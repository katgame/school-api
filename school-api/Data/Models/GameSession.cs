using school_api.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace school_api.Data.Models
{
    public class GameSession
    {
        public Guid SessionId { get; set; }
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid AvatarId { get; set; }
        public ConnectionStatus isPlayerActive { get; set; }
        public DateTime LastPingResponse { get; set; }
        public DateTime GameSessionStartTime { get; set; }
       
        public int Duration { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Tokens { get; set; }
        public Avatar Avatar { get; set; }


    }
}
