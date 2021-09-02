using EuroBooks.Application.Common.Interfaces;

namespace EuroBooks.Infrastructure.Configuration
{
    public class JwtConfiguration : IJwtConfiguration
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
    }
}
