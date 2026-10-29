namespace Flowin.contracts
{
    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public RegisterRequest() { }
        public RegisterRequest(string name, string surname, string email, string password)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
        }
    }
}
