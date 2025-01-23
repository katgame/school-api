using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace school_api.Enums
{
    public enum ConnectionStatus
    {
        [Display(Name= "#4ccb19, Green")]
        Active,
        [Display(Name = "#B11a03, Red")]
        Offline,
        [Display(Name = "#Cb4419, Orange")]
        Pending
    }

}
