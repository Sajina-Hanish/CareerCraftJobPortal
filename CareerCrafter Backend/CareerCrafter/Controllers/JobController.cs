﻿using CareerCrafter.DTOs;
using CareerCrafter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerCrafter.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService) => _jobService = jobService;

        [HttpGet]
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> GetAll() => Ok(await _jobService.GetAllJobsAsync());

        [HttpGet("search")]
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> Search([FromQuery] string title, [FromQuery] string location)
        {
            return Ok(await _jobService.SearchJobsAsync(title, location));
        }

        [HttpGet("employer")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> GetEmployerJobs()
        {
            int employerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _jobService.GetEmployerJobsAsync(employerId));
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Create(JobDTO dto)
        {
            int employerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _jobService.CreateJobAsync(employerId, dto));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Update(int id, JobDTO dto)
        {
            int employerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _jobService.UpdateJobAsync(employerId, id, dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Delete(int id)
        {
            int employerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _jobService.DeleteJobAsync(employerId, id));
        }
    }
}
