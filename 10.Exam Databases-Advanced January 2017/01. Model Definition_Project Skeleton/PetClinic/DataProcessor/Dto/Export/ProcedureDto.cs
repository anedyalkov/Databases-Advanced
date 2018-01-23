using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Export
{
    [XmlType("Procedure")]
    public class ProcedureDto
    {
        public string Passport { get; set; }

        public string OwnerNumber { get; set; }

        public string DateTime { get; set; }

        public AnimalAidDto[] AnimalAids { get; set; } = new AnimalAidDto[0];

        public decimal TotalPrice { get; set; }
    }
}
