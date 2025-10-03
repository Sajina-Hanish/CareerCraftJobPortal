using System.ComponentModel.DataAnnotations;

namespace CareerCrafter.Models
{
    public class JobApplication
    {
        public int jobApplicationId { get; set; }

        public int jobId { get; set; }
        public Job Job { get; set; }

        public int JobSeekerId { get; set; }
        public User JobSeeker { get; set; }

        [Required (ErrorMessage = "Please upload resume")]
        public string ResumePath { get; set; }

        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
    }
}
