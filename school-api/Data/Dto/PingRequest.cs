using System;

namespace school_api.Data.Dto
{
    public class PingRequest
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public Guid GameSessionId { get; set; }
    }
}
