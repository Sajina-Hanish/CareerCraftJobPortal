using CareerCrafter.Data;
using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CareerCrafter.Tests
{
    public class JobServiceTests
    {
        private JobService _jobService;
        private ApplicationDbContext _db;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("JobDb").Options;

            _db = new ApplicationDbContext(options);
            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();

            _jobService = new JobService(_db);
        }

        [Test]
        public async Task CreateJobAsync_ShouldAddJob()
        {
            var dto = new JobCreateDTO
            {
                Title = "Software Dev",
                Description = "Write code",
                Company = "Tech Inc",
                Salary = 50000
            };

            var result = await _jobService.CreateJobAsync(dto, employerId: 1);

            Assert.NotNull(result);
            Assert.AreEqual("Software Dev", result.Title);
            Assert.AreEqual(1, result.EmployerId);
        }

        [Test]
        public async Task UpdateJobAsync_ShouldModifyJob()
        {
            var job = new Job { Title = "Old", Description = "Desc", Company = "OldCo", Salary = 1000, EmployerId = 5 };
            _db.Jobs.Add(job);
            _db.SaveChanges();

            var dto = new JobCreateDTO { Title = "New", Description = "NewDesc", Company = "NewCo", Salary = 8000 };

            var updated = await _jobService.UpdateJobAsync(job.Id, dto, employerId: 5);

            Assert.NotNull(updated);
            Assert.AreEqual("New", updated.Title);
        }

        [Test]
        public async Task DeleteJobAsync_ShouldRemoveJob()
        {
            var job = new Job { Title = "Del", Description = "Test", Company = "Co", Salary = 2000, EmployerId = 2 };
            _db.Jobs.Add(job);
            _db.SaveChanges();

            var deleted = await _jobService.DeleteJobAsync(job.Id, employerId: 2);
            Assert.IsTrue(deleted);
        }

        [Test]
        public async Task GetApplicationCountAsync_ShouldReturnCount()
        {
            var job = new Job { Title = "X", Description = "Y", Company = "Z", Salary = 10000, EmployerId = 9 };
            _db.Jobs.Add(job);
            _db.SaveChanges();

            _db.JobApplications.Add(new JobApplication { JobId = job.Id, JobSeekerId = 10, ResumePath = "file.pdf" });
            _db.SaveChanges();

            var count = await _jobService.GetApplicationCountAsync(job.Id, employerId: 9);

            Assert.AreEqual(1, count);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

    }
}
