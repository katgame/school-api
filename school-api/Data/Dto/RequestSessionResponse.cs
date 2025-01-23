using school_api.Data.Models;
using System;

namespace school_api.Data.Dto
{
    public class RequestSessionResponse
    {
        public Guid SchoolId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SessionStartTime { get; set; }
        public bool isNewSession { get; set; }
        public Session SessionInfo { get; set; }



    }


}
