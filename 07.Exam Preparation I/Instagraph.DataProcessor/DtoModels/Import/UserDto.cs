namespace Instagraph.DataProcessor.DtoModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserDto
    {
        [MaxLength(30)]
        [Required]
        public string Username { get; set; }

        [MaxLength(20)]
        [Required]
        public string Password { get; set; }

        [Required]
        public string ProfilePicture { get; set; }
    }
}
