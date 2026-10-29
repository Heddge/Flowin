using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowin.Interfaces;

namespace Flowin.Objects
{
    public class LocalSessionContainer : NotesContainer
    {
        public User CurrentUser { get; set; }

        public LocalSessionContainer(ISynchronizeManager sm) : base(sm)
        {
            CurrentUser = new User("Egor", "Test", "egorik@test.ru");
            CurrentUser.Email = "egor@test.ru";
        }
        public User GetUser() => CurrentUser;
    }
}
