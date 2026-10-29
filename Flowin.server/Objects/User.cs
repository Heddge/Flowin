using Microsoft.AspNetCore.Identity;

namespace Flowin.server.Objects
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public override string Email { get; set; } = "";
        public override string UserName { get; set; } = "";
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
