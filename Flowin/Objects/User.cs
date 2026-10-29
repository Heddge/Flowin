using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Flowin.Objects
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public User() { }

        public User(string name, string sname)
        {
            Name = name;
            Surname = sname;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
