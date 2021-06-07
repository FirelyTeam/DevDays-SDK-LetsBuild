using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
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
            Identifier = new List<Identifier> { new Identifier("system", "example") },
            Gender = AdministrativeGender.Unknown,
            Name = new List<HumanName> { new HumanName { Family = "Visser" } },
            Active = true,
            BirthDate = "2001-03-01",
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


            var client = new FhirClient("https://server.fire.ly/r4");

            // retrieve the capability statement of the FHIR Server
            var capability = client.CapabilityStatement();
            Console.WriteLine($"capability: Name: {capability.Name}, Fhir Version: {capability.FhirVersion} ");

            try
            {
                var oo = client.ValidateResource(_patient);
                Console.WriteLine(oo.ToJson(jsonSerializationSettings));

                var pat = client.Create<Patient>(_patient);

                Console.WriteLine($"Id of patient: {pat.Id}");

                // retrieve the patient again from the server:
                var patFromServer = client.Read<Patient>($"Patient/{pat.Id}"); // or Read<Patient>(ResourceIdentity.Build("Patient", pat.Id));
                Console.WriteLine($"Read patient from server with id : {pat.Id}\n{patFromServer.ToJson(jsonSerializationSettings)}");

                // search functions:
                Bundle results = client.Search<Patient>(new string[] { "family:exact=Visser" });
                Console.WriteLine($"Search patient with family name exact to 'Visser'\n{results.ToJson(jsonSerializationSettings)}");

                // search by id:
                results = client.SearchById<Patient>(pat.Id);
                Console.WriteLine($"Search patient by Id\n{results.ToJson(jsonSerializationSettings)}");

                // using the class SearchParams
                var q = new SearchParams()
                    .Where("family:exact=Visser")
                    .OrderBy("birthdate", SortOrder.Descending)
                    .SummaryOnly().Include("Patient:organization")
                    .LimitTo(5);
                results = client.Search<Patient>(q);
                Console.WriteLine($"Complex search\n{results.ToJson(jsonSerializationSettings)}");

                // continue with the next page:
                results = client.Continue(results);
                Console.WriteLine($"Next page\n{results.ToJson(jsonSerializationSettings)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something has happened: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
