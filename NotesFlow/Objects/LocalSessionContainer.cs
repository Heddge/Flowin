using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotesFlow.Interfaces;

namespace NotesFlow.Objects
{
    public class LocalSessionContainer : NotesContainer
    {
        private Guid _userId { get; set; }
        private string _name { get; set; } = "";
        private string _surname { get; set; } = "";
        private string _email { get; set; }

        public LocalSessionContainer(ISynchronizeManager sm) : base(sm)
        {
            _userId = Guid.NewGuid();
            _name = "";
            _surname = "";
            _email = "";
        }

        public Guid GetUserId() => _userId;

    }
}
