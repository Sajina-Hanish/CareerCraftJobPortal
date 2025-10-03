using CareerCrafter.DTOs;
using CareerCrafter.Models;

namespace CareerCrafter.Repositories.Interfaces
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(int employerId, JobDTO dto);
        Task<List<Job>> GetEmployerJobsAsync(int employerId);
        Task<List<Job>> GetAllJobsAsync();
        Task<Job> GetJobByIdAsync(int id);
        Task<Job> UpdateJobAsync(int employerId, int jobId, JobDTO dto);
        Task<bool> DeleteJobAsync(int employerId, int jobId);
        Task<List<Job>> SearchJobsAsync(string title, string location);
    }
}
