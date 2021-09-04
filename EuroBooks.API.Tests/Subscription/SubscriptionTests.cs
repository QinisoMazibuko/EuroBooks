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
using System.Text;
using System.Threading.Tasks;

namespace EuroBooks.API.Tests.Subscription
{
    using static Testing;
    public class SubscriptionTests:TestBase
    {

        private readonly HttpClient client;
        private readonly TestServer server;

        public SubscriptionTests()
        {
            server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            client = server.CreateClient();
            client.BaseAddress = new Uri("https://localhost:5001");
        }


        [TestCase("superadmin@user", "Admin")]
        [TestCase("Subsciber@user", "Subscriber")]
        public async Task User_Should_Login(string userName, string role)
        {
            await Login(userName, role);
        }

        [TestCase("Subsciber@user", "Subscriber")]
        public async Task User_Should_getSubscriptions(string userName, string role)
        {
            // Arrange
            await Login(userName, role);

            var book1 = await AddAsync(new Domain.Enities.Book
            {
                Name = "Harry Potter",
                Text = "best book of all time",
                PurchasePrice = 560,
            });

            var book2 = await AddAsync(new Domain.Enities.Book
            {
                Name = "Harry Potter 2",
                Text = "best book of all time",
                PurchasePrice = 360,
            });


            var book3 = await AddAsync(new Domain.Enities.Book
            {
                Name = "Harry Potter 3",
                Text = "best book of all time",
                PurchasePrice = 460,
            });

            await AddRangeAsync(new Domain.Enities.Subscription
            {
                UserID = 1,
                BookID = book1.Id,
            }, new Domain.Enities.Subscription
            {
                UserID = 1,
                BookID = book2.Id,
            }, new Domain.Enities.Subscription
            {
                UserID = 1,
                BookID = book3.Id,
            });

            // Act
            var response = await client.GetAsync($"/api/subscription/list/{1}");

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
