using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RehberCore.Models
{
    public class Number : BaseModel
    {
        public string PhoneNumber { get; set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
