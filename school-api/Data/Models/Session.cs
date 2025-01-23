using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace school_api.Data.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }
        public bool Active { get; set; }

        public List<GameSession> GameSession { get; set; } = new List<GameSession>();
        //public School School { get; set; } = null!;
    }
}
