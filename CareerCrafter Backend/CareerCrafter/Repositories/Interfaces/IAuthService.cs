using CareerCrafter.DTOs;
using CareerCrafter.Models;

namespace CareerCrafter.Repositories.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDTO dto);
        Task<string> LoginAsync(LoginDTO dto);
        Task<bool> UserExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
