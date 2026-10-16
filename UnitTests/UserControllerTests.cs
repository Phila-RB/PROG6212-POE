
using FakeItEasy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PROGPOE.Controllers;
using PROGPOE.Models;
using System.Timers;

namespace UnitTests
{
    public class UserControllerTests
    {

        private readonly UserController _controller;

        public UserControllerTests()
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:RaceDayCon"] =
                    "Server=(localdb)\\MSSQLLocalDB;Database=RaceDayTest;Trusted_Connection=True;TrustServerCertificate=True"
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _controller = new UserController(config);

            [Fact]
            public void RegisterFail()
            {
                // Arrange
                User user = null!;

                // Act
                var result = _controller.Register(user);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
            // for a non-null user, so this test needs a test database.



        } }
}


