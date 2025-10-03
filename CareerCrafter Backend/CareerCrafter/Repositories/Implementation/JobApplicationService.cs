using CareerCrafter.Data;
using CareerCrafter.DTOs;
using CareerCrafter.Models;
using CareerCrafter.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareerCrafter.Repositories.Implementation
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public JobApplicationService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<JobApplication> ApplyAsync(int jobSeekerId, ApplicationDTO dto)
        {
            try
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Resume.FileName);
                var relativePath = Path.Combine("resumes", fileName);
                var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.Resume.CopyToAsync(stream);
                }

                var application = new JobApplication
                {
                    jobId = dto.jobId,
                    JobSeekerId = jobSeekerId,
                    ResumePath = fileName
                };

                _context.JobApplications.Add(application);
                await _context.SaveChangesAsync();
                return application;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while applying for the job: " + ex.Message);
            }
        }

        public async Task<List<JobApplication>> GetMyApplicationsAsync(int jobSeekerId)
        {
            try
            {
                return await _context.JobApplications
                    .Include(a => a.Job)
                    .Where(a => a.JobSeekerId == jobSeekerId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch job applications: " + ex.Message);
            }
        }

        public async Task<List<JobApplication>> GetApplicationsForEmployerAsync(int employerId)
        {
            try
            {
                return await _context.JobApplications
                    .Include(a => a.Job)
                    .Include(a => a.JobSeeker)
                    .Where(a => a.Job.EmployerId == employerId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve applications for employer: " + ex.Message);
            }
        }

        public async Task<string> DeleteApplicationAsync(int jobSeekerId, int applicationId)
        {
            try
            {
                var application = await _context.JobApplications
                    .FirstOrDefaultAsync(a => a.jobApplicationId == applicationId && a.JobSeekerId == jobSeekerId);

                if (application == null)
                    return "Delete failed (The application may not be yours)";

                _context.JobApplications.Remove(application);
                await _context.SaveChangesAsync();
                return "Delete successful";
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the application: " + ex.Message);
            }
        }
    }
}
