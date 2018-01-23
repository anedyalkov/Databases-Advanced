using System.ComponentModel.DataAnnotations;

namespace Instagraph.DataProcessor.DtoModels
{
    public class UserFollowerDto
    {
        [MaxLength(30)]
        [Required]
        public string User { get; set; }

        [MaxLength(30)]
        [Required]
        public string Follower { get; set; }
    }
}
