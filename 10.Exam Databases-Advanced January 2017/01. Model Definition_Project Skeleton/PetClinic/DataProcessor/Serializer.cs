namespace PetClinic.DataProcessor
{
    using System;

    using PetClinic.Data;
    using System.Xml;
    using Newtonsoft.Json;
    using System.Linq;
    using PetClinic.DataProcessor.Dto.Export;
    using System.Globalization;
    using Formatting = Newtonsoft.Json.Formatting;
    using System.Xml.Serialization;
    using System.Text;
    using System.IO;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {

            var animals = context.Animals
                .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .Select(a => new AnimalDto
                {
                    OwnerName = a.Passport.OwnerName,
                    AnimalName = a.Name,
                    Age = a.Age,
                    SerialNumber = a.Passport.SerialNumber,
                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy")
                })
                .OrderBy(a => a.Age)
                .ThenBy(a => a.SerialNumber)
                .ToArray();

            var json = JsonConvert.SerializeObject(animals,Formatting.Indented);
            return json;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {

            var procedures = context.Procedures
                .OrderBy(p => p.DateTime)
                .ThenBy(p => p.Animal.PassportSerialNumber)
                .Select(p => new
                {
                    Passport = p.Animal.PassportSerialNumber,
                    OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                    DateTime = p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    AnimalAids = p.ProcedureAnimalAids
                    .Select(pa => new AnimalAidDto()
                    {
                        Name = pa.AnimalAid.Name,
                        Price = pa.AnimalAid.Price
                    }).ToArray(),
                    TotalPrice = p.ProcedureAnimalAids.Sum(pa => pa.AnimalAid.Price)
                })
                .Select(p => new ProcedureDto
                {
                    Passport = p.Passport,
                    OwnerNumber = p.OwnerNumber,
                    DateTime = p.DateTime,
                    AnimalAids = p.AnimalAids,
                    TotalPrice = p.TotalPrice
                })
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));
            serializer.Serialize(new StringWriter(sb), procedures, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var result = sb.ToString();
            return result;
        }
    }
}
