using school_api.Data.Models;

using System;
using System.Collections.Generic;

namespace school_api.Data.Dto
{
    public class DashBoardResponse
    {
        public List<int> LastWeekStats { get; set; }
        public List<int> CurrentWeekStats { get; set; }
        public bool increase { get; set; }
        public double Growth { get; set; }
        public double LastWeekTotal { get; set; }
        public double CurrentWeekTotal { get; set; }

    }
}
