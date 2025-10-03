using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using CareerCrafter.Repositories.Implementation;
using CareerCrafter.Controllers;
using CareerCrafter.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CareerCrafter.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AuthController _authController;
        private Mock<IAuthService> _authMock;
        private IConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _authMock = new Mock<IAuthService>();

            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "xYu9Nq4!sVr@j7KzLd8#Wh2pBmA&EfQz"}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authController = new AuthController(_authMock.Object, _config);
        }

        [Test]
        public async Task RegisterAsync_ShouldRegisterUser()
        {
            var dto = new RegisterDTO
            {
                Email = "test@email.com",
                Password = "123456",
                Role = "JobSeeker"
            };


            _authMock
                .Setup(r => r.RegisterAsync(It.IsAny<RegisterDTO>()))
                .ReturnsAsync("User registered successfully");

            var result = await _authController.Register(dto); 

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task RegisterAsync_ShouldThrowIfEmailExists()
        {

            var dto = new RegisterDTO
            {
                Email = "exist@email.com",
                Password = "pass",
                Role = "Employer"
            };

            _authMock
                .Setup(r => r.UserExistsAsync(dto.Email))
                .ReturnsAsync(true);

            var result = await _authController.Register(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task LoginAsync_ShouldThrowOnInvalidCreds()
        {
            var dto = new LoginDTO
            {
                Email = "notfound@gmail.com",
                Password = "wrong123"
            };

            _authMock
                .Setup(u => u.GetUserByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            var result = await _authController.Login(dto);

            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }
    }
}
