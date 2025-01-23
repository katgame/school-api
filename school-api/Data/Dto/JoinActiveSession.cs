using System;

namespace school_api.Data.Dto
{
    public class JoinActiveSessionRequest
    {
        public string schoolId { get; set; }
        public string UserId { get; set; }
        public string gameType { get; set; }

    }
}
