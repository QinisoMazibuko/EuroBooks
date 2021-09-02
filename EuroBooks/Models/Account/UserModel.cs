using System.Collections.Generic;


namespace EuroBooks.API.Models.Account
{
    public class UserModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public IList<string> UserRoles { get; set; }
    }
}
