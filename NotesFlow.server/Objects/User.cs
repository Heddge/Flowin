using Microsoft.AspNetCore.Identity;

namespace NotesFlow.server.Objects
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
