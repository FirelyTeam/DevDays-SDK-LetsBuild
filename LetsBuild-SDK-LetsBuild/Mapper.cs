using Hl7.Fhir.Model;

namespace LetsBuild
{
    public class Mapper
    {
        public Mapper()
        {
        }

        public Patient GetPatient(CSVModel record)
        {
            Patient patient = new Patient();

            return patient;
        }

        public Observation GetWhiteBloodCellCount(CSVModel record)
        {
            // White blood cell count - This corresponds to LOINC code:
            // Code:        6690-2
            // Display:     Leukocytes [#/volume] in Blood by Automated count
            // Unit System: http://unitsofmeasure.org
            // Unit Code:   10*3/uL

            Observation wbc = new Observation();
   
            return wbc;
        }

        public Observation GetRedBloodCellCount(CSVModel record)
        {
            // Red blood cell count - This corresponds to LOINC code:
            // Code:        789-8
            // Display:     Erythrocytes [#/volume] in Blood by Automated count
            // Unit System: http://unitsofmeasure.org
            // Unit Code:   10*6/uL

            Observation rbc = new Observation();

            return rbc;
        }

        public Observation GetHemoglobin(CSVModel record)
        {
            // Hemoglobin
            // Code:        718-7
            // Display:     Hemoglobin [Mass/volume] in Blood
            // Unit System: http://unitsofmeasure.org
            // Unit Code:   g/dL

            Observation hb = new Observation();

            return hb;
        }
    }
}
