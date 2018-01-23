namespace PetClinic.DataProcessor
{
    using System;

    using PetClinic.Data;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;
    using PetClinic.DataProcessor.Dto.Import;
    using PetClinic.Models;
    using System.Linq;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.IO;

    public class Deserializer
    {
        private const string FailureMessage = "Error: Invalid data.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedAnimalAids = JsonConvert.DeserializeObject<AnimalAidDto[]>(jsonString);

            var validAnimalAids = new List<AnimalAid>();

            foreach (var animalAidsDto in deserializedAnimalAids)
            {
                if (!IsValid(animalAidsDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var animalAidExists = validAnimalAids.Any(a => a.Name == animalAidsDto.Name);

                if (animalAidExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var animalAid = new AnimalAid()
                {
                    Name = animalAidsDto.Name,
                    Price = animalAidsDto.Price
                };

                validAnimalAids.Add(animalAid);
                sb.AppendLine(string.Format(SuccessMessage, animalAidsDto.Name));
            }

            context.AnimalAids.AddRange(validAnimalAids);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedAnimals = JsonConvert.DeserializeObject<AnimalDto[]>(jsonString);

            var validAnimals = new List<Animal>();

            foreach (var animalDto in deserializedAnimals)
            {

                if (!IsValid(animalDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var animalSerialNumberExist = validAnimals.Any(a => a.Passport.SerialNumber == animalDto.Passport.SerialNumber);

                if (animalSerialNumberExist)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var date = DateTime.ParseExact(animalDto.Passport.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                //Passport passport = context.Passports.SingleOrDefault(p => p.SerialNumber == animalDto.Passport.SerialNumber);

               
                   var passport = new Passport()
                    {
                        SerialNumber = animalDto.Passport.SerialNumber,
                        OwnerName = animalDto.Passport.OwnerName,
                        OwnerPhoneNumber = animalDto.Passport.OwnerPhoneNumber,
                        RegistrationDate = date,
                        Animal = new Animal()
                        {
                            Name = animalDto.Name,
                            Type = animalDto.Type,
                            Age = animalDto.Age,
                        }
                    };

                    if (!IsValid(passport))
                    {
                        sb.AppendLine(FailureMessage);
                        continue;
                    }           

                var animal = new Animal()
                {
                    Name = animalDto.Name,
                    Type = animalDto.Type,
                    Age = animalDto.Age,
                    Passport = passport
                };

                validAnimals.Add(animal);
                sb.AppendLine($"Record { animal.Name} Passport №: { animal.Passport.SerialNumber} successfully imported.");
            }

            context.Animals.AddRange(validAnimals);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(VetDto[]), new XmlRootAttribute("Vets"));
            var deserializedVets = (VetDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var sb = new StringBuilder();

            var validVets = new List<Vet>();

            foreach (var vetDto in deserializedVets)
            {
                if (!IsValid(vetDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var vetsPhoneNumberExists = validVets.Any(v => v.PhoneNumber == vetDto.PhoneNumber);

                if (vetsPhoneNumberExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var vet = new Vet()
                {
                    Name = vetDto.Name,
                    Profession = vetDto.Profession,
                    Age = vetDto.Age,
                    PhoneNumber = vetDto.PhoneNumber,

                };

                validVets.Add(vet);

                sb.AppendLine(string.Format(SuccessMessage, vetDto.Name));
            }

            context.Vets.AddRange(validVets);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));
            var deserializedProcedures = (ProcedureDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var sb = new StringBuilder();

            var validProcedures = new List<Procedure>();

            foreach (var procedureDto in deserializedProcedures)
            {
                if (!IsValid(procedureDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var validAnimalAid = procedureDto.AnimalAids.All(IsValid);

                if (!validAnimalAid)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var vetExists = context.Vets.Any(v => v.Name == procedureDto.Vet);

                if (!vetExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var animalSerialNumberExists = context.Animals.Any(a => a.PassportSerialNumber == procedureDto.Animal);

                if (!animalSerialNumberExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var procedureContainsExistingAnimalAids = procedureDto.AnimalAids.All(aa => context.AnimalAids.Any(a => a.Name == aa.Name));

                if (!procedureContainsExistingAnimalAids)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                var dateTime = DateTime.ParseExact(procedureDto.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var procedure = new Procedure()
                {
                    Animal = context.Animals.SingleOrDefault(x => x.PassportSerialNumber == procedureDto.Animal),
                    Vet = context.Vets.SingleOrDefault(x => x.Name == procedureDto.Vet),
                    DateTime = dateTime
                };

                var procedureAnimalAids = new List<ProcedureAnimalAid>();

                var animalAidNameIsRepeating = false;

                foreach (var animalAidDto in procedureDto.AnimalAids)
                {
                    if (procedureAnimalAids.Any(pa => pa.AnimalAid.Name == animalAidDto.Name))
                    {
                        sb.AppendLine(FailureMessage);
                        animalAidNameIsRepeating = true;
                        break;
                    }

                    var procedureAnimalAid = new ProcedureAnimalAid()
                    {
                        Procedure = procedure,
                        AnimalAid = context.AnimalAids.SingleOrDefault(aa => aa.Name == animalAidDto.Name)
                    };

                    procedureAnimalAids.Add(procedureAnimalAid);
                }

                if (animalAidNameIsRepeating)
                {
                    continue;
                }

                procedure.ProcedureAnimalAids = procedureAnimalAids;

                validProcedures.Add(procedure);
                sb.AppendLine("Record successfully imported.");
            }
            context.Procedures.AddRange(validProcedures);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            return isValid;
        }
    }
}
