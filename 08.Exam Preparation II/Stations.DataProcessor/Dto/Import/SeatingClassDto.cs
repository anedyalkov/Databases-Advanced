namespace Stations.DataProcessor.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class SeatingClassDto
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [StringLength(2, MinimumLength = 2)]
        public string Abbreviation { get; set; }
    }
}
