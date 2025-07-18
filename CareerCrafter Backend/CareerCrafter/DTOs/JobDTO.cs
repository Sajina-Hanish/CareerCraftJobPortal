﻿using System.ComponentModel.DataAnnotations;

namespace CareerCrafter.DTOs
{
    public class JobDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public string Qualification { get; set; }

        [Required]
        public int Salary { get; set; }
    }
}
