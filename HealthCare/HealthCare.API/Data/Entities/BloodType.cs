﻿using System.ComponentModel.DataAnnotations;

namespace HealthCare.API.Data.Entities
{
    public class BloodType
    {
        public int Id { get; set; }
        
        [Display (Name = "Blood type")]
        [MaxLength(50, ErrorMessage = "The field {0} cannot have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Description { get; set; }
    }
}
