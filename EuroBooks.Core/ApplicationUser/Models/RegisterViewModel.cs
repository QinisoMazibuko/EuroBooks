namespace EuroBooks.Core.ApplicationUser.Models
{
    public class RegisterViewModel
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }

    public class UserViewmodel
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Id { get; set; }

        public string Role { get; set; }

    }
}
