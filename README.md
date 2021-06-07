# Let's Build .Net Fall 2020

The instructions for day 1 can also be found [here](https://github.com/FirelyTeam/LetsBuildNetFall2020/blob/main/DD20_Nov_Track_Day1.pdf) in pdf form.

# Let’s build a FHIR app - .Net

## Day 1 - Build a FHIR Data Mapper

**Rationale**: When adopting FHIR, a common scenario is needing to convert your existing data into the FHIR
model. This can be a challenging first step, but if you approach it systematically it can be easy.

**Exercise**: For this exercise, we will be building a mapper that converts existing data a CSV file into FHIR Patient
and Observation resources. We will use the [Firely .NET SDK](https://github.com/FirelyTeam/firely-net-sdk) for the FHIR models.

We'll be using a sample data file from the CDC NHANES (National Health and Nutrition Examination Study)
publicly available sample data set. The format of the data set is described at this link but James Agnew has
reworked the format a bit to add fake patient identities and timestamps to the data.

The input CSV file can be found [here](https://github.com/FirelyTeam/LetsBuildNetFall2020/blob/day-1/sample-data.csv): sample-data.csv

**Approach**
The input data looks like the following:
```
SEQN   ,TIMESTAMP               ,PATIENT_ID,PATIENT_FAMILYNAME,PATIENT_GIVENNAME,PATIENT_GENDER,WBC,RBC,HB
93704.0,2020-11-13T07:47:35.964Z,PT00002   ,Simpson           ,Marge            ,F             ,7.4,0.1,13.1
```

Note the columns:
- SEQN: This is a unique identifier for the test
- TIMESTAMP: This is the timestamp for the test
- Patient detail columns (note that the patients repeat so you will want to ):
  * PATIENT_ID: This is the ID of the patient
  * PATIENT_FAMILYNAME: This is the family (last) name of the patient
  * PATIENT_GIVENNAME: This is the given (first) name of the patient
  * PATIENT_GENDER: This is the gender of the patient
- Test result columns (each of these will be a separate Observation resource):
  - WBC: "White Blood Cells": This a count of the number
  - RBC: "Red Blood Cells"
  - HB: "Hemoglobin"


**Information for mapping the Observations**:

White blood cell count - This corresponds to LOINC code:
```
Code: 6690-2
Display: Leukocytes [#/volume] in Blood by Automated count
Unit System: http://unitsofmeasure.org
Unit Code: 10*3/uL
```
Red blood cell count - This corresponds to LOINC code:
```
Code: 789-8
Display: Erythrocytes [#/volume] in Blood by Automated count
Unit System: http://unitsofmeasure.org
Unit Code 10*6/uL
```
Hemoglobin:
```
Code: 718-7
Display: Hemoglobin [Mass/volume] in Blood
Unit System: http://unitsofmeasure.org
Unit Code: g/dL
```

When mapping the Observations, please take a look at http://hl7.org/fhir/observation.html for the relevant
fields to put this data in, and to check which fields are mandatory.

Have fun, and remember to ask for help if you get stuck!
