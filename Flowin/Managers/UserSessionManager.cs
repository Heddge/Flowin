using System.Text.Json;
using Flowin.Objects;

namespace Flowin.Managers
{
    public class UserSessionManager
    {
        public bool IsAuthorized { get; set; } = false;
        public User AuthorizedUser { get; set; } = new User("", "", "", Guid.Empty);
        public UserSessionManager() { }

        public void SetAuthorizedUser(User authed_user)
        {
            this.IsAuthorized = true;
            this.AuthorizedUser = authed_user;
            Preferences.Set("AuthorizedUser", JsonSerializer.Serialize(AuthorizedUser));
        }

        public async Task Unauthorize()
        {
            this.IsAuthorized = false;
            this.AuthorizedUser = new User("", "", "", Guid.Empty);
            if (Preferences.ContainsKey("AuthorizedUser"))
                Preferences.Remove("AuthorizedUser");
        }

        public async Task RestoreAuthorization()
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

        public async Task<User> GetUser()
        {
            await RestoreAuthorization();
            return this.AuthorizedUser;
        }
    }
}
