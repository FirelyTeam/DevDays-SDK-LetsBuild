using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Hl7.Fhir.Model;

namespace LetsBuild2021June
{
    class Program
    {
        static void Main(string[] args)
        {

            TextReader reader = new StreamReader("sample-data.csv");
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csvReader.GetRecords<CSVModel>();

            var patientList = new List<Patient>();
            var observationList = new List<Observation>();

            var map = new Mapper();
            foreach (var r in records)
            {
                var pat = map.GetPatient(r);
                if (!patientList.Exists(p => p.Id == pat.Id))
                    patientList.Add(pat);

                observationList.Add(map.GetWhiteBloodCellCount(r));
                observationList.Add(map.GetRedBloodCellCount(r));
                observationList.Add(map.GetHemoglobin(r));
            }

            DisplayData(patientList, observationList);
        }

        static void DisplayData(List<Patient> patients, List<Observation> observations)
        {
            // This can be greatly improved, but just functions as an example of obtaining data from
            // a FHIR resource and displaying that


            Console.WriteLine($"There are {patients.Count} patients in the file. Here is their data:");

            foreach (var p in patients)
            {
                Console.WriteLine("-----------------------------------------------------------------------------");

                Console.WriteLine("Data for patient: " + p.Name.First().Given.First() + " " + p.Name.First().Family);

                var patsObservations = observations.FindAll(o => o.Subject.Reference == "Patient/" + p.Id);

                // Split the observations based on type
                var wbcCode = new CodeableConcept("http://loinc.org", "6690-2");
                var rbcCode = new CodeableConcept("http://loinc.org", "789-8");
                var hbCode = new CodeableConcept("http://loinc.org", "718-7");

                var wbcObservations = patsObservations.FindAll(o => o.Code.Matches(wbcCode));
                var rbcObservations = patsObservations.FindAll(o => o.Code.Matches(rbcCode));
                var hbObservations = patsObservations.FindAll(o => o.Code.Matches(hbCode));

                Console.WriteLine("  WBC Observations:");
                foreach (var obs in wbcObservations)
                {
                    var qty = (Quantity)obs.Value;
                    Console.WriteLine($"    {obs.Effective}  {qty.Value} {qty.Code}");
                }
                Console.WriteLine("  RBC Observations:");
                foreach (var obs in rbcObservations)
                {
                    var qty = (Quantity)obs.Value;
                    Console.WriteLine($"    {obs.Effective}  {qty.Value} {qty.Code}");
                }
                Console.WriteLine("  HB Observations:");
                foreach (var obs in hbObservations)
                {
                    var qty = (Quantity)obs.Value;
                    Console.WriteLine($"    {obs.Effective}  {qty.Value} {qty.Code}");
                }
            }
            Console.WriteLine("-----------------------------------------------------------------------------");

        }
    }
}
