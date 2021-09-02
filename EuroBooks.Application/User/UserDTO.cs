using System;
using System.Collections.Generic;

namespace EuroBooks.Application.User
{
    public class UserDTO
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public IList<string> Roles { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? LastModified { get; set; }
  
    }
}
