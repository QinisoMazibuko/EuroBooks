using EuroBooks.API.Models;
using EuroBooks.API.Models.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EuroBooks.API.Tests.Account
{
    using static Testing;
    public class AccountTests : TestBase
    {
        private readonly HttpClient client;
        private readonly TestServer server;

        public AccountTests()
        {
            server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            client = server.CreateClient();
            client.BaseAddress = new Uri("https://localhost:5001");
        }

        [TestCase("support@EuroBooks.co.za", "Admin")]
        public async Task Account_Should_Login(string userName, string role)
        {
            await Login(userName, role);
        }

        [TestCase("support@EuroBooks.co.za", "Admin")]
        public async Task Account_Should_PostUser(string userName, string role)
        {
            // Arrange
            await Login(userName, role);

            var content = new UserModel { UserName = "jack@test.co", Email = "jack@test.co", FirstName = "Jack", LastName = "Green", Password = "JackTest1!", UserRoles = new List<string> {"Admin"} };

            // Act
            var response = await client.PostAsync($"/api/account/user", ContentHelper.GetStringContent(content));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        #region Private Methods
        private async Task Login(string userName, string role)
        {
            // Arrange
            await RunAsUserAsync(userName, "Test@123", "Test", "User", role);

            var content = new LoginModel { Username = userName, Password = "Test@123" };

            // Act
            var response = await client.PostAsync("/api/authorize/login", ContentHelper.GetStringContent(content));

            //Assert 
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

            result.ShouldNotBeNull();
            result.Token.ShouldNotBeNull();

            // Set Token
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
        }
        #endregion
    }
}
