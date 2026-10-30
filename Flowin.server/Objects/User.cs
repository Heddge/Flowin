using Microsoft.AspNetCore.Identity;

namespace Flowin.server.Objects
{
    public class User : IdentityUser<Guid>
    {
        public override Guid Id {  get; set; }
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public override string Email { get; set; } = "";
        public override string UserName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public User() { }
        public User(string name, string sname, 
            string email, Guid id)
        {
            Id = id;
            Name = name;
            Surname = sname;
            Email = email;
            UserName = Id.ToString().Replace('-', 'o');
            CreatedAt = DateTime.UtcNow;
        }
    }
}
