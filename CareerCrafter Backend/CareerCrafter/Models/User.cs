using System.ComponentModel.DataAnnotations;

namespace CareerCrafter.Models
{
    public class User
    {
        public int userId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be ")]
        public string Password { get; set; }

        [Required, RegularExpression("Employer|JobSeeker", ErrorMessage = "Role must be Employer or JobSeeker")]
        public string Role { get; set; }

        public ICollection<Job> Jobs { get; set; }
        public ICollection<JobApplication> Applications { get; set; }
    }
}
