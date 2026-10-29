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
            CurrentUser = new User("Egor", "Test");
            CurrentUser.Email = "egor@test.ru";
            CurrentUser.UserName = "egorTest";
        }
        public User GetUser() => CurrentUser;
    }
}
