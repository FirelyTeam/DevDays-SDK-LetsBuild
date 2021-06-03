using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using System;
using System.Collections.Generic;

namespace LetsBuild2021June
{
    public class Program
    {
        private static Patient _patient = new()
        {
            Meta = new Meta()
            {
                Profile = new[] { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient" }
            },
            Id = "1",
            Identifier = new List<Identifier> { new Identifier("system", "example") },
            Gender = AdministrativeGender.Unknown,
            Name = new List<HumanName> { new HumanName { Family = "Visser" } },
            Active = true,
            BirthDate = "2001-03-01"
        };

        public static void Main(string[] args)
        {
            // pretty print the json
            var jsonSerializationSettings = new FhirJsonSerializationSettings { Pretty = true };

            // Set up resolver
            var resolver = new CachedResolver(new MultiResolver(
                new DirectorySource("profiles", new DirectorySourceSettings { IncludeSubDirectories = true }),
                ZipSource.CreateValidationSource()
            ));

            // setting up the validator
            ValidationSettings settings = new() { ResourceResolver = resolver };
            Validator validator = new(settings);

            // validate the patient
            var outcome = validator.Validate(_patient);

            // print the outcome
            Console.WriteLine($"Success: {outcome.Success} \n{outcome.ToJson(jsonSerializationSettings)}");

            Console.ReadKey();
        }
    }
}
