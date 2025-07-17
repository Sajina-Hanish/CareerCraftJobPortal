using CareerCrafter.Data;
using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CareerCrafter.Tests
{
    public class JobApplicationServiceTests
    {
        private ApplicationDbContext _context;
        private JobApplicationService _service;
        private Mock<IWebHostEnvironment> _envMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("JobAppDb")
                .Options;

            _context = new ApplicationDbContext(options);

            _envMock = new Mock<IWebHostEnvironment>();
            _envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _service = new JobApplicationService(_context, _envMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task ApplyAsync_ShouldSaveResumeAndReturnApplication()
        {
            // Arrange
            var job = new Job
            {
                Id = 1,
                EmployerId = 2,
                Title = "Dev",
                Description = "Test",
                Company = "TestCo",
                Location = "BLR",
                Salary = 40000,
                Qualification = "B.Tech"
            };

            var user = new User
            {
                Id = 1,
                Email = "seeker@gmail.com",
                Password = "pwd123",
                Role = "JobSeeker"
            };

            _context.Jobs.Add(job);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var resumeBytes = Encoding.UTF8.GetBytes("Fake resume content");
            var resumeMock = new Mock<IFormFile>();
            resumeMock.Setup(f => f.FileName).Returns("resume.pdf");
            resumeMock.Setup(f => f.Length).Returns(resumeBytes.Length);
            resumeMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(resumeBytes));
            resumeMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, CancellationToken>((stream, token) =>
                {
                    var memoryStream = new MemoryStream(resumeBytes);
                    return memoryStream.CopyToAsync(stream, token);
                });

            var dto = new ApplicationDTO
            {
                JobId = job.Id,
                Resume = resumeMock.Object
            };

            // Act
            var result = await _service.ApplyAsync(user.Id, dto);

            // Assert
            var savedPath = Path.Combine(_envMock.Object.WebRootPath, "resumes", result.ResumePath);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(File.Exists(savedPath), Is.True, $"Resume file not found at: {savedPath}");
        }

        [Test]
        public async Task GetMyApplicationsAsync_ShouldReturnOnlyMyApps()
        {
            var user = new User
            {
                Id = 10,
                Email = "me@gmail.com",
                Password = "123456",
                Role = "JobSeeker"
            };

            var job = new Job
            {
                Id = 20,
                EmployerId = 1,
                Title = "QA",
                Description = "test",
                Company = "Co",
                Location = "IN",
                Salary = 1000,
                Qualification = "Any" 
            };

            _context.Users.Add(user);
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            var app = new JobApplication
            {
                JobId = job.Id,
                JobSeekerId = user.Id,
                ResumePath = "fake.pdf"
            };

            _context.JobApplications.Add(app);
            await _context.SaveChangesAsync();

            var apps = await _service.GetMyApplicationsAsync(user.Id);
            Assert.That(apps, Has.Exactly(1).Items);
        }

        [Test]
        public async Task DeleteApplicationAsync_ShouldRemoveApp()
        {
            var app = new JobApplication
            {
                Id = 10,
                JobId = 1,
                JobSeekerId = 3,
                ResumePath = "delete.pdf"
            };

            _context.JobApplications.Add(app);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteApplicationAsync(3, 10);

            Assert.That(result, Is.EqualTo("Delete successful"));
            Assert.That(await _context.JobApplications.FindAsync(10), Is.Null);
        }

        [Test]
        public async Task DeleteApplicationAsync_ShouldReturnFailureMessage_WhenNotOwned()
        {
            var app = new JobApplication
            {
                Id = 11,
                JobId = 1,
                JobSeekerId = 99,
                ResumePath = "notyours.pdf"
            };

            _context.JobApplications.Add(app);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteApplicationAsync(3, 11);

            Assert.That(result, Is.EqualTo("Delete failed (The application may not be yours)"));
        }
    }
}
