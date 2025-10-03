using CareerCrafter.Controllers;
using CareerCrafter.Data;
using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Repositories.Implementation;
using CareerCrafter.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CareerCrafter.Tests
{
    public class JobApplicationServiceTests
    {
        private Mock<IJobApplicationService> _jobappMock;
        private JobApplicationController _jobAppController;

        [SetUp]
        public void Setup()
        {
            _jobappMock = new Mock<IJobApplicationService>();

            _jobAppController = new JobApplicationController(_jobappMock.Object);
        }

        private void User(JobApplicationController controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "mock");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }


        [Test]
        public async Task ApplyAsync_Should_SaveResumeAndReturnApplication()
        {
            // Arrange
            var jobApp = new JobApplication
            {
                jobApplicationId = 1,
                jobId = 1,
                JobSeekerId = 1,
                ResumePath = "resume.pdf"
            };

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

            var dto = new ApplicationDTO { jobId = 1, Resume = resumeMock.Object };

            _jobappMock.Setup(r => r.ApplyAsync(1, dto)).ReturnsAsync(jobApp);

            // Fake user with JobSeeker role
            User(_jobAppController, 1, "JobSeeker");

            // Act
            var result = await _jobAppController.Apply(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _jobappMock.Verify(r => r.ApplyAsync(1, dto), Times.Once);
        }

        [Test]
        public async Task GetMyApplicationsAsync_Should_ReturnOnlyMyApplications()
        {
            // Arrange
            var apps = new List<JobApplication>
            {
                new JobApplication { jobApplicationId = 1, jobId = 20, JobSeekerId = 10, ResumePath = "fake.pdf" }
            };

            _jobappMock.Setup(r => r.GetMyApplicationsAsync(10)).ReturnsAsync(apps);

            User(_jobAppController, 10, "JobSeeker");

            // Act
            var result = await _jobAppController.MyApplications();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedApps = okResult.Value as List<JobApplication>;
            Assert.That(returnedApps, Has.Exactly(1).Items);
            _jobappMock.Verify(r => r.GetMyApplicationsAsync(10), Times.Once);
        }

        [Test]
        public async Task DeleteApplicationAsync_Should_RemoveApplication()
        {
            // Arrange
            _jobappMock.Setup(r => r.DeleteApplicationAsync(3, 10)).ReturnsAsync("Delete successful");

            User(_jobAppController, 3, "JobSeeker");

            // Act
            var result = await _jobAppController.Delete(10);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Delete successful"));
            _jobappMock.Verify(r => r.DeleteApplicationAsync(3, 10), Times.Once);
        }

        [Test]
        public async Task DeleteApplicationAsync_Should_ReturnFailureMessage_WhenNotOwned()
        {
            // Arrange
            _jobappMock.Setup(r => r.DeleteApplicationAsync(3, 11))
                .ReturnsAsync("Delete failed (The application may not be yours)");

            User(_jobAppController, 3, "JobSeeker");

            // Act
            var result = await _jobAppController.Delete(11);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Delete failed (The application may not be yours)"));
            _jobappMock.Verify(r => r.DeleteApplicationAsync(3, 11), Times.Once);
        }

    }
}
