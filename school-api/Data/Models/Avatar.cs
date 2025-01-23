using System;

namespace school_api.Data.Models
{
    public class Avatar
    {
        public Guid Id { get; set; }
        public int OrderId { get; set; }
        public string LeftPosition { get; set; }
        public string TopPosition { get; set; }
        public string Image { get; set; }
        public string StrokeColor { get; set; }
        public bool Reverse { get; set; }

    }
}
