using CareerCrafter.Data;
using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareerCrafter.Repositories.Implementation
{
    public class JobService : IJobService
    {
        private readonly AppDbContext _context;

        public JobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Job> CreateJobAsync(int employerId, JobDTO dto)
        {
            try
            {
                var job = new Job
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Location = dto.Location,
                    Company = dto.Company,
                    Qualification = dto.Qualification,
                    Salary = dto.Salary,
                    EmployerId = employerId
                };

                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();
                return job;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create job", ex);
            }
        }

        public async Task<List<Job>> GetEmployerJobsAsync(int employerId)
        {
            try
            {
                return await _context.Jobs
                    .Where(j => j.EmployerId == employerId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve employer jobs", ex);
            }
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            try
            {
                return await _context.Jobs.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve jobs", ex);
            }
        }

        public async Task<Job> GetJobByIdAsync(int id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                    throw new Exception("Job not found");

                return job;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve job", ex);
            }
        }

        public async Task<Job> UpdateJobAsync(int employerId, int jobId, JobDTO dto)
        {
            try
            {
                var job = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.jobId == jobId && j.EmployerId == employerId);

                if (job == null)
                    throw new Exception("Job not found or access denied");

                job.Title = dto.Title;
                job.Description = dto.Description;
                job.Location = dto.Location;
                job.Company = dto.Company;
                job.Qualification = dto.Qualification;
                job.Salary = dto.Salary;

                await _context.SaveChangesAsync();
                return job;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update job", ex);
            }
        }

        public async Task<bool> DeleteJobAsync(int employerId, int jobId)
        {
            try
            {
                var job = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.jobId == jobId && j.EmployerId == employerId);

                if (job == null)
                    return false;

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete job", ex);
            }
        }

        public async Task<List<Job>> SearchJobsAsync(string title, string location)
        {
            try
            {
                return await _context.Jobs
                    .Where(j =>
                        (string.IsNullOrEmpty(title) || j.Title.Contains(title)) &&
                        (string.IsNullOrEmpty(location) || j.Location.Contains(location)))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to search jobs", ex);
            }
        }
    }
}
