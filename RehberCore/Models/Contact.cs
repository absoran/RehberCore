using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RehberCore.Models
{
    public class Contact : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Number> Numbers { get; set; }
    }
}
