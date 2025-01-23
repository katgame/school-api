
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace school_api.Data.Models
{
    public class Address
    {

        public Guid Id { get; set; }
        public string StreetName { get; set; }
        public string Line1 {get;set;}
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

    }
}
