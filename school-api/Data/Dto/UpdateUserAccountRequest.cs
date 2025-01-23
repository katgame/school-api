using school_api.Enums;
using System;

namespace school_api.Data.Dto
{
    public class UpdateUserAccountRequest
    {
        public Guid UserId { get; set; }
        public double Price { get; set; }
        public PlayerGameSessionResult GameSessionResult { get; set; }
      
    }
}
