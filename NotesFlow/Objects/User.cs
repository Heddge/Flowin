using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesFlow.Objects
{
    public class User
    {
        private Guid _id { get; set; }
        private string _name { get; set; }
        private string? _surname { get; set; }
        private string _email { get; set; }
        private DateTime _createdAt { get; set; }
        private List<Note> _notes { get; set; } = new List<Note>();

        public User() { }

        public User(string Name, string? Sname, string Emal)
        {
            _id = Guid.NewGuid();
            _name = Name;
            _surname = Sname;
            _email = Emal;
            _notes = new List<Note>();
        }
    }
}
