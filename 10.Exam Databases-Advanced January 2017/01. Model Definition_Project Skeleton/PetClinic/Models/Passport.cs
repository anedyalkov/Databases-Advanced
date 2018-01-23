namespace PetClinic.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Passport
    {
        [Key]
        [RegularExpression(@"[a-zA-Z]{7}\d{3}$")]
        public string SerialNumber { get; set; }

        [Required]
        public Animal Animal { get; set; }

        [Required]
        [RegularExpression(@"^\+359\d{9}$|^0[0-9]{9}$")]
       //RegularExpression(@"^0\d{9}$")]
        public string OwnerPhoneNumber { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string OwnerName { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

    }
}