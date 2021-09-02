using System;


namespace EuroBooks.API.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
    }
}
