using Flowin.Objects;
using Flowin.Interfaces;

namespace Flowin.Managers
{
    public class AuthenticationManager
    {
        INotesApiManager notesApiManager;
        public AuthenticationManager(INotesApiManager inam)
        {
            notesApiManager = inam;
        }

        public void Registration(string name, string surname, 
            string email, string password, string second_password)
        {
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(second_password))
                return;

            if (password != second_password) return;

            if (!email.Contains('@') && !email.Contains('.'))
                return;

            User new_user = new User(name, surname);
            new_user.Email = email;
            new_user.UserName = email + "us";

            notesApiManager.CreateUser(new_user, password);
        }
    }
}
