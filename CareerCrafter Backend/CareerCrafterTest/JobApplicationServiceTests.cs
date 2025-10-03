using CareerCrafter.Data;
using CareerCrafter.Models;
using CareerCrafter.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CareerCrafter.Tests
{
    public class JobApplicationServiceTests
    {
        private ApplicationDbContext _db;
        private JobApplicationService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("JobAppDb").Options;

            _db = new ApplicationDbContext(options);
            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();

            _service = new JobApplicationService(_db);
        }

        [Test]
        public async Task ApplyToJobAsync_ShouldSaveApplicationAndResume()
        {
            var job = new Job { Title = "Test", Description = "Job", Company = "Comp", Salary = 6000, EmployerId = 1 };
            _db.Jobs.Add(job);
            _db.SaveChanges();

            var resumeMock = new Mock<IFormFile>();
            var content = "fake resume content";
            var fileName = "resume.pdf";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            resumeMock.Setup(f => f.OpenReadStream()).Returns(stream);
            resumeMock.Setup(f => f.FileName).Returns(fileName);
            resumeMock.Setup(f => f.Length).Returns(stream.Length);
            resumeMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream target, System.Threading.CancellationToken _) =>
            {
                stream.CopyTo(target);
                return Task.CompletedTask;
            });

            string tempPath = Path.Combine(Path.GetTempPath(), "resumes");
            var result = await _service.ApplyToJobAsync(job.Id, 100, resumeMock.Object, tempPath);

            Assert.NotNull(result);
            Assert.AreEqual(job.Id, result.JobId);
            Assert.AreEqual(100, result.JobSeekerId);
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, result.ResumePath.Replace("/resumes/", ""))));
        }

        [Test]
        public async Task GetApplicationsForEmployerAsync_ShouldReturnList()
        {
            var job = new Job { Title = "A", Description = "B", Company = "C", Salary = 1000, EmployerId = 999 };
            _db.Jobs.Add(job);
            _db.SaveChanges();

            _db.JobApplications.Add(new JobApplication { JobId = job.Id, JobSeekerId = 1, ResumePath = "x.pdf" });
            _db.SaveChanges();

            var result = await _service.GetApplicationsByEmployerAsync(999);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task GetApplicationsByJobSeekerAsync_ShouldReturnList()
        {
            var job = new Job { Title = "Q", Description = "W", Company = "E", Salary = 1000, EmployerId = 10 };
            _db.Jobs.Add(job);
            _db.SaveChanges();

            _db.JobApplications.Add(new JobApplication { JobId = job.Id, JobSeekerId = 500, ResumePath = "z.pdf" });
            _db.SaveChanges();

            var result = await _service.GetApplicationsByJobSeekerAsync(500);
            Assert.AreEqual(1, result.Count);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

    }
}
