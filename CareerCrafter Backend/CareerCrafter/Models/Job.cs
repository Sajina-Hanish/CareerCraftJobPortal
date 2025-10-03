using System.ComponentModel.DataAnnotations;

namespace CareerCrafter.Models
{
    public class Job
    {
        public int jobId { get; set; }

        [Required (ErrorMessage = "Enter job title")]
        public string Title { get; set; }

        [Required (ErrorMessage = "Enter job descripton")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Enter job location")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Enter hiring company")]
        public string Company { get; set; }

        [Required(ErrorMessage = "Enter qualification required")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Enter job salary")]
        public int Salary { get; set; }

        public int EmployerId { get; set; }
        public User Employer { get; set; }

        public ICollection<JobApplication> Applications { get; set; }
    }
}
