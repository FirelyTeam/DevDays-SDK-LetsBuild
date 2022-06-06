namespace LetsBuildProject
{
    public class CSVModel
    {

        //SEQN,TIMESTAMP,PATIENT_ID,PATIENT_FAMILYNAME,PATIENT_GIVENNAME,PATIENT_GENDER,WBC,RBC,HB
        public string SEQN { get; set; }
        public string TIMESTAMP { get; set; }
        public string PATIENT_ID { get; set; }
        public string PATIENT_FAMILYNAME { get; set; }
        public string PATIENT_GIVENNAME { get; set; }
        public string PATIENT_GENDER { get; set; }
        public string WBC { get; set; }
        public string RBC { get; set; }
        public string HB { get; set; }


        public CSVModel()
        { }
    }
}
