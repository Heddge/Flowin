using Flowin.Interfaces;
using Flowin.Objects;

namespace Flowin.Managers
{
    public class AuthenticationManager
    {
        INotesApiManager notesApiManager;
        public AuthenticationManager(INotesApiManager inam)
            => notesApiManager = inam;

        public async Task Registration(string name, string surname, 
            string email, string password, string second_password)
        {
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(second_password))
                return;

            if (password != second_password) return;

            if (!email.Contains('@') || !email.Contains('.'))
                return;

            User new_user = new User(name, surname, email);
            new_user.Email = email;

            await notesApiManager.CreateUser(new_user, password);
        }

        public async Task<User?> Logining(string email, string password)
        {
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password))
                return null;

            var response = await notesApiManager.Login(email, password);
            if (response == null) return null;
            return response.data;
        }
    }
}
