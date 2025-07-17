using CareerCrafter.Data;
using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerCrafter.Tests
{
    public class AuthServiceTests
    {
        private AuthService _authService;
        private ApplicationDbContext _dbContext;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            // In-memory DB
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthDb")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            // Clear previous data
            _dbContext.Users.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();

            // Mock IConfiguration
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForTesting");
            mockConfig.Setup(c => c["Jwt:ValidIssuer"]).Returns("CareerCrafterAPI");
            mockConfig.Setup(c => c["Jwt:ValidAudience"]).Returns("CareerCrafterClient");

            _configuration = mockConfig.Object;
            _authService = new AuthService(_dbContext, _configuration);
        }

        [Test]
        public async Task RegisterAsync_ShouldAddNewUser()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@example.com",
                Password = "123456",
                Role = "JobSeeker"
            };

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.AreEqual("Registration successful", result);
            Assert.IsTrue(await _dbContext.Users.AnyAsync(u => u.Email == dto.Email));
        }

        [Test]
        public void RegisterAsync_ShouldThrowIfEmailExists()
        {
            // Arrange
            _dbContext.Users.Add(new User { Email = "existing@example.com", Password = "pwd", Role = "Employer" });
            _dbContext.SaveChanges();

            var dto = new RegisterDTO
            {
                Email = "existing@example.com",
                Password = "newpass",
                Role = "JobSeeker"
            };

            // Act & Assert
            Assert.ThrowsAsync<System.Exception>(async () => await _authService.RegisterAsync(dto));
        }

        [Test]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User
            {
                Email = "valid@example.com",
                Password = "pass123",
                Role = "Employer"
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var dto = new LoginDTO { Email = "valid@example.com", Password = "pass123" };

            // Act
            var token = await _authService.LoginAsync(dto);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsTrue(token.Length > 0);
        }

        [Test]
        public void LoginAsync_ShouldThrow_WhenInvalidCredentials()
        {
            // Arrange
            var dto = new LoginDTO { Email = "wrong@example.com", Password = "badpass" };

            // Act & Assert
            Assert.ThrowsAsync<System.Exception>(async () => await _authService.LoginAsync(dto));
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}
