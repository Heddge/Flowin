using System.Text.Json;
using Flowin.Objects;

namespace Flowin.Managers
{
    public class UserSessionManager
    {
        public bool IsAuthorized { get; set; } = false;
        public User AuthorizedUser { get; set; }
        public UserSessionManager()
        {
            AuthorizedUser = new User();
            RestoreAuthorization();
        }

        public void SetAuthorizedUser(User authed_user)
        {
            this.IsAuthorized = true;
            this.AuthorizedUser = authed_user;
            Preferences.Set("AuthorizedUser", JsonSerializer.Serialize(AuthorizedUser));
        }

        public void Unauthorize()
        {
            this.IsAuthorized = false;
            this.AuthorizedUser = new User();
            if (Preferences.ContainsKey("AuthorizedUser"))
                Preferences.Clear("AutorizedUser");
        }

        public void RestoreAuthorization()
        {
            if (Preferences.ContainsKey("AuthorizedUser"))
            {
                string? userInJson = Preferences.Get("AuthorizedUser", null);
                if (userInJson != null)
                {
                    this.AuthorizedUser = JsonSerializer.Deserialize<User>(userInJson);
                    this.IsAuthorized = true;
                }
            }
        }
    }
}
