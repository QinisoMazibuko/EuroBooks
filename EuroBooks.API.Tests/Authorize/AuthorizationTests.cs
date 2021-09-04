using EuroBooks.API.Models;
using EuroBooks.API.Models.Account;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EuroBooks.API.Tests.Authorize
{
    using static Testing;

    /// <summary>
    /// Test for Authorization
    /// </summary>
    public class AuthorizationTests : TestBase
    {

        public AuthorizationTests()
        {

        }

        [Test]
        public async Task Should_LogUserIn()
        {
            // Arrange
            await RunAsUserAsync("test@gmail.com", "Testing@123", "Test", "User", "Admin");

            var content = new LoginModel { Username = "test@gmail.com", Password = "Testing@123" };

            // Act
            var response = await client.PostAsync("/api/authorize/login", ContentHelper.GetStringContent(content));

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

            result.ShouldNotBeNull();
            result.Token.ShouldNotBeNull();
        }
    }
}
