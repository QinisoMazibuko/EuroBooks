using System.ComponentModel.DataAnnotations;


namespace EuroBooks.API.Models.Account
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
