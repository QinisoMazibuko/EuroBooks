using EuroBooks.API.Models;
using EuroBooks.API.Models.Account;
using EuroBooks.Application.Book.Commands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EuroBooks.API.Tests.Book
{
    using static Testing;
    public class BookTests: TestBase
    {
        private readonly HttpClient client;
        private readonly TestServer server;

        public BookTests()
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

        [TestCase("superadmin@user", "Admin")]
        [TestCase("Subsciber@user", "Subscriber")]
        public async Task user_Should_GetBooks(string userName, string role)
        {
            // Arrange
            await Login(userName, role);

            await AddRangeAsync(new Domain.Enities.Book 
            { 
                Name = "Harry Potter",
                Text = "best book of all time",
                PurchasePrice = 560,
            },new Domain.Enities.Book 
            { 
                Name = "Rich Dad Poor Dad",
                Text = "Self coaching book",
                PurchasePrice = 60,
            },new Domain.Enities.Book 
            { 
                Name = "Tess of the D'urbavilles ",
                Text = "a really great read for english titerature",
                PurchasePrice = 700,
            },new Domain.Enities.Book 
            { 
                Name = "7 habbits of highly effective individuls",
                Text = "another life coaching book :)",
                PurchasePrice = 900,
            });
   
            // Act 
            var response = await client.GetAsync($"/api/book/listAll");

            // Assert
            response.EnsureSuccessStatusCode();
            response.ShouldNotBeNull();
        }

        [TestCase("superadmin@user", "Admin")]
        public async Task user_Should_AddBook(string userName, string role)
        {
            // Arrange
            await Login(userName, role);

            var content = new CreateBookCommand
            {
                Name = "7 habbits of highly effective individuls",
                Text = "another life coaching book :)",
                PurchasePrice = 900,
            };

            // Act
            var response = await client.PostAsync($"/api/book", ContentHelper.GetStringContent(content));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [TestCase("superadmin@user", "Admin")]
        [TestCase("Subsciber@user", "Subscriber")]
        public async Task user_Should_GetBook(string userName, string role)
        {
            // Arrange
            await Login(userName, role);

            var book = await AddAsync(new Domain.Enities.Book
            {
                Name = "Harry Potter 5",
                Text = "best book of all time",
                PurchasePrice = 500,
            });
            var bookId = book.Id;

            // Act
            var response = await client.GetAsync($"/api/book/{bookId}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.ShouldNotBeNull();
        }

        [TestCase("superadmin@user", "Admin")]
        public async Task Admin_Should_EditBook(string userName, string role)
        {
            // Arrange
            await Login(userName, role);

            var book = await AddAsync(new Domain.Enities.Book
            {
                Name = "Harry Potter",
                Text = "best book of all time",
                PurchasePrice = 560,
            });
            var bookId = book.Id;


            var content = new UpdateBookCommand
            {
                Id = bookId,
                Name = "Harry Potter 2",
                Text = "best book of all time continued",
                PurchasePrice = 560,
            };

            // Act
            var response = await client.PutAsync($"/api/book/{bookId}", ContentHelper.GetStringContent(content));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [TestCase("superadmin@user", "Admin")]
        public async Task Admin_Should_DeleteBook(string userName, string role)
        {
            // Arrange
            await Login(userName, role);

            var book = await AddAsync(new Domain.Enities.Book
            {
                Name = "Harry Potter 3",
                Text = "best book of all time",
                PurchasePrice = 560,
                IsActive = true
            });

            var bookId = book.Id;
            // Act

            var response = await client.DeleteAsync($"/api/book/{bookId}");

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
