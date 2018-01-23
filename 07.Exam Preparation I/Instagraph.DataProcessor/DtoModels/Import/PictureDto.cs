using System.ComponentModel.DataAnnotations;

namespace Instagraph.DataProcessor.DtoModels
{
    public class PictureDto
    {
        [Required]
        public string Path { get; set; }

        public decimal Size { get; set; }
    }
}
