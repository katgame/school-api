using System;

namespace school_api.Data.Dto
{
    public class SessionRequest
    {
        public Guid SchoolId { get; set; }
        public Guid UserId { get; set; }
    }
}
