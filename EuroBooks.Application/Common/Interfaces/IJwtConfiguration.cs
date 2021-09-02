using System;

namespace EuroBooks.Application.Common.Interfaces
{
    public interface IJwtConfiguration
    {
        string Secret { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
        string Subject { get; set; }
    }
}
