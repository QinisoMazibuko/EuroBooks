using EuroBooks.Domain.Enities;
using EuroBooks.Infrastructure.Identity;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EuroBooks.API.Tests
{
    using static Testing;
    public class TestBase
    {
        [SetUp]
        public async Task TestSetUp()
        {
            await ResetState();

            await AddRangeAsync(new ApplicationRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            }, new ApplicationRole
            {
                Name = "Subscriber",
                NormalizedName = "SUBSCRIBER"
            });

            // Settings
            await AddAsync(new ApplicationVariables { Key = "SMTPServer", Value = "smtp.gmail.com", Description = "SMTP Server" });
            await AddAsync(new ApplicationVariables { Key = "SMTPUser", Value = "tester.eurobooks@gmail.com", Description = "SMTP User" });
            await AddAsync(new ApplicationVariables { Key = "SMTPPassword", Value = "P@ssword3#", Description = "SMTP Password" });
            await AddAsync(new ApplicationVariables { Key = "SMTPPort", Value = "587", Description = "SMTP Port" });
            await AddAsync(new ApplicationVariables { Key = "SMTPFromEmail", Value = "tester.eurobooks@gmail.com", Description = "SMTP From Email" });
            await AddAsync(new ApplicationVariables { Key = "SMTPFromName", Value = "EuroBooks", Description = "SMTP From Name" });
        }
    }
}
