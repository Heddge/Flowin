using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Flowin.Objects
{
    public class User
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public User() { }

        public User(string name, string sname,
            string email)
        {
            Name = name;
            Surname = sname;
            Email = email;
            UserName = email + name + "UsName";
            CreatedAt = DateTime.UtcNow;
        }
    }
}
