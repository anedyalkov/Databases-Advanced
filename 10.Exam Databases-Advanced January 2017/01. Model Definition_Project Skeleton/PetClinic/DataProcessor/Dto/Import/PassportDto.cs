﻿using System.ComponentModel.DataAnnotations;

namespace PetClinic.DataProcessor.Dto.Import
{

    public class PassportDto
    {
        [Required]
        [RegularExpression(@"[a-zA-Z]{7}\d{3}$")]
        public string SerialNumber { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string OwnerName { get; set; }

        [Required]
        [RegularExpression(@"^\+359\d{9}$|^0[0-9]{9}$")]
        public string OwnerPhoneNumber { get; set; }

        [Required]
        public string RegistrationDate { get; set; }
    }
}