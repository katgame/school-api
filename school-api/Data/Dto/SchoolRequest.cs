using System;

namespace school_api.Data.Dto
{
    public class SchoolRequest
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxPlayers { get; set; }
        public double BuyInFee { get; set; }
        public double Price { get; set; }
        public bool Active { get; set; }
    }
}
