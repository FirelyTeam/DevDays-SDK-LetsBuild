using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            }

            Console.WriteLine($"There are {patientList.Count} patients in the file");
        }
    }
}
