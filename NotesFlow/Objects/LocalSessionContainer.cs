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
        public User CurrentUser { get; set; }
        private Guid _userId { get; set; }

        public LocalSessionContainer(ISynchronizeManager sm) : base(sm)
        {
            CurrentUser = new User("Egor", "Test");
            CurrentUser.Email = "egor@test.ru";
            CurrentUser.UserName = "egorTest";
            _userId = CurrentUser.Id;
        }

        public Guid GetUserId() => _userId;

    }
}
