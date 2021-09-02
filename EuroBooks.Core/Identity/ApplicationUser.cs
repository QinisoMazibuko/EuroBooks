using Microsoft.AspNetCore.Identity;
using System;

namespace EuroBooks.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
