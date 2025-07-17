using CareerCrafter.DTOs;
using CareerCrafter.Models;

namespace CareerCrafter.Repositories
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDTO dto);
        Task<string> LoginAsync(LoginDTO dto);
    }
}
