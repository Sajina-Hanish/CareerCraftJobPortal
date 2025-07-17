using CareerCrafter.DTOs;
using CareerCrafter.Models;

namespace CareerCrafter.Repositories
{
    public interface IJobApplicationService
    {
        Task<JobApplication> ApplyAsync(int jobSeekerId, ApplicationDTO dto);
        Task<List<JobApplication>> GetMyApplicationsAsync(int jobSeekerId);
        Task<List<JobApplication>> GetApplicationsForEmployerAsync(int employerId);
        Task<string> DeleteApplicationAsync(int jobSeekerId, int applicationId);
    }
}
