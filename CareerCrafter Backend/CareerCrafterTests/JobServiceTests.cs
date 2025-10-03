using CareerCrafter.Controllers;
using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CareerCrafter.Tests
{
    public class JobServiceTests
    {
        private Mock<IJobService> _jobMock;
        private JobController _jobController;

        [SetUp]
        public void Setup()
        {
            _jobMock = new Mock<IJobService>();
            _jobController = new JobController(_jobMock.Object);
        }

        private void User(JobController controller, int employerId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employerId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "mock");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }


        [Test]
        public async Task CreateJobAsync_Should_AddJob()
        {
            // Arrange 

            var dto = new JobDTO
            {
                Title = "Dev",
                Description = "Build stuff",
                Location = "Chennai",
                Company = "Hexaware",
                Salary = 50000,
                Qualification = "B.Tech"
            };

            var fakeJob = new Job
            {
                jobId = 1,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Company = dto.Company,
                Salary = dto.Salary,
                Qualification = dto.Qualification,
                EmployerId = 1
            };

            _jobMock
                .Setup(s => s.CreateJobAsync(1, dto))
                .ReturnsAsync(fakeJob);


            User(_jobController, 1, "Employer");

            // Act
            var result = await _jobController.Create(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }


        [Test]
        public async Task GetAllJobsAsync_Should_ReturnList()
        {
            // Arrange
            var fakeJobs = new List<Job>
            {
                new Job { jobId = 1, Title = "QA", Location = "Mumbai", Company = "Testers Inc", Salary = 30000, EmployerId = 2, Qualification = "Any degree" }
            };

            _jobMock
                .Setup(s => s.GetAllJobsAsync())
                .ReturnsAsync(fakeJobs);

            // Act
            var result = await _jobController.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteJobAsync_Should_ReturnFalseForInvalidJob()
        {

            User(_jobController, 2, "Employer");

            // Arrange
            _jobMock
                .Setup(s => s.DeleteJobAsync(1, 999))
                .ReturnsAsync(false);


            // Act
            var result = await _jobController.Delete(999);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}
