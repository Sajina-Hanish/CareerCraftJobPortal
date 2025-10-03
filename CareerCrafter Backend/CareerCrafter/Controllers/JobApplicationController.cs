using CareerCrafter.DTOs;
using CareerCrafter.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerCrafter.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _service;

        public JobApplicationController(IJobApplicationService service) => _service = service;

        [HttpPost]
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> Apply([FromForm] ApplicationDTO dto)
        {
            int jobSeekerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _service.ApplyAsync(jobSeekerId, dto));
        }

        [HttpGet("my")]
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> MyApplications()
        {
            int jobSeekerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _service.GetMyApplicationsAsync(jobSeekerId));
        }

        [HttpDelete("{jobApplicationId}")]
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> Delete(int jobApplicationId)
        {
            int jobSeekerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _service.DeleteApplicationAsync(jobSeekerId, jobApplicationId));
        }

        [HttpGet("employer")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> GetForEmployer()
        {
            int employerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var apps = await _service.GetApplicationsForEmployerAsync(employerId);
            return Ok(apps);
        }
    }
}
