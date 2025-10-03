using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CareerCrafter.DTOs
{
    public class ApplicationDTO
    {
        [Required]
        public int jobId { get; set; }

        [Required]
        public IFormFile Resume { get; set; }
    }
}
