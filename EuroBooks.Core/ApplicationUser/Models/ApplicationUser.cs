using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EuroBooks.Core.Shared
{
    public class ApplicationUser: IdentityUser
    {
        [Column(TypeName = "nvarchar(150)")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

    }
}

