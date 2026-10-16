using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PROGPOE.Controllers;
using PROGPOE.Models;

namespace UnitTests;

public class UserControllerTests
{
        private readonly UserController _controller;

        public UserControllerTests()
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:RaceDayCon"] =
                    "Server=localhost;Database=RaceDay_Test;" +
                    "Trusted_Connection=True;TrustServerCertificate=True;"
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _controller = new UserController(config);
        }

        [Fact]
        public void Register_NullUser_ReturnsNotFound()
        {
            // Arrange
            User user = null!;

            // Act
            var result = _controller.Register(user);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateProfile_NullUser_ReturnsNotFound()
        {
            // Arrange
            User user = null!;

            // Act
            var result = _controller.UpdateProfile(user);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void SignUP_NullSignup_ReturnsNotFound()
        {
            // Arrange
            Signup signup = null!;

            // Act
            var result = _controller.SignUP(1, signup);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }



