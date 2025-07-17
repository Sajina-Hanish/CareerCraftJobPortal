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
        private ApplicationDbContext _context;
        private JobService _jobService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("JobDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _jobService = new JobService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateJobAsync_ShouldAddJob()
        {
            var dto = new JobDTO
            {
                Title = "Dev",
                Description = "Build stuff",
                Location = "Chennai",
                Company = "Hexaware",
                Salary = 50000,
                Qualification = "B.Tech" 
            };

            var job = await _jobService.CreateJobAsync(1, dto);

            Assert.That(job.Id, Is.GreaterThan(0));
            Assert.That(job.EmployerId, Is.EqualTo(1));
            Assert.That(job.Qualification, Is.EqualTo("B.Tech"));
        }

        [Test]
        public async Task GetAllJobsAsync_ShouldReturnList()
        {
            _context.Jobs.Add(new Job
            {
                Title = "QA",
                Description = "Test apps",
                Location = "Mumbai",
                Company = "Testers Inc",
                Salary = 30000,
                EmployerId = 2,
                Qualification = "Any degree" 
            });
            await _context.SaveChangesAsync();

            var jobs = await _jobService.GetAllJobsAsync();

            Assert.That(jobs, Is.Not.Empty);
        }

        [Test]
        public async Task DeleteJobAsync_ShouldReturnFalseForInvalidJob()
        {
            var result = await _jobService.DeleteJobAsync(1, 999);
            Assert.False(result);
        }
    }
}
