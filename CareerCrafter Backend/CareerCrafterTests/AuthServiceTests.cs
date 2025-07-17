using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Repositories;
using CareerCrafter.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerCrafter.Tests
{
    public class AuthServiceTests
    {
        private AuthService _authService;
        private ApplicationDbContext _context;
        private IConfiguration _config;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("AuthDb")
                .Options;
            _context = new ApplicationDbContext(options);

            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "xYu9Nq4!sVr@j7KzLd8#Wh2pBmA&EfQz"}
            };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authService = new AuthService(_context, _config);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
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

            var result = await _authService.RegisterAsync(dto);

            Assert.That(result, Is.EqualTo("User registered successfully"));
        }

        [Test]
        public void RegisterAsync_ShouldThrowIfEmailExists()
        {
            _context.Users.Add(new User
            {
                Email = "exist@email.com",
                Password = "pass",
                Role = "Employer"
            });
            _context.SaveChanges();

            var dto = new RegisterDTO
            {
                Email = "exist@email.com",
                Password = "pass",
                Role = "Employer"
            };

            Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(dto));
        }

        [Test]
        public async Task LoginAsync_ShouldReturnToken()
        {
            _context.Users.Add(new User
            {
                Email = "login@test.com",
                Password = "pwd",
                Role = "Employer"
            });
            _context.SaveChanges();

            var dto = new LoginDTO
            {
                Email = "login@test.com",
                Password = "pwd"
            };

            var token = await _authService.LoginAsync(dto);

            Assert.That(token, Is.Not.Null.And.Contains("."));
        }

        [Test]
        public void LoginAsync_ShouldThrowOnInvalidCreds()
        {
            var dto = new LoginDTO
            {
                Email = "notfound@gmail.com",
                Password = "wrong123"
            };

            Assert.ThrowsAsync<InvalidOperationException>(() => _authService.LoginAsync(dto));
        }
    }
}
