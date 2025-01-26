using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Pin { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public int PhoneNumber { get; set; }
    }
}
