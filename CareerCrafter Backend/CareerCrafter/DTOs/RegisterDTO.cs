using System.ComponentModel.DataAnnotations;

namespace CareerCrafter.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Employer|JobSeeker", ErrorMessage = "Role must be either 'Employer' or 'JobSeeker'")]
        public string Role { get; set; } = string.Empty;
    }
}
