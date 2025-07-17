using CareerCrafter.Data;
using CareerCrafter.DTOs;
using CareerCrafter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareerCrafter.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> RegisterAsync(RegisterDTO dto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                    throw new InvalidOperationException("Email already exists");

                var user = new User
                {
                    Email = dto.Email,
                    Password = dto.Password,
                    Role = dto.Role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return "User registered successfully";
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during registration.", ex);
            }
        }

        public async Task<string> LoginAsync(LoginDTO dto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

                if (user == null)
                    throw new InvalidOperationException("Invalid credentials");

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during login.", ex);
            }
        }
    }
}
