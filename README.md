# Let's Build .Net Session 2

The instructions for session 2 can also be found [here](https://github.com/FirelyTeam/DevDays-SDK-LetsBuild/blob/session2/DD22_FHIRApp_session2.pdf) in pdf form.

## Session 2a – Validating FHIR resources

**Exercise**: For this exercise, we are going to validate our resources before sending them to a FHIR server. We will use the Validator of the Firely .NET SDK.

### Exercise steps
-	Create a new C# Console project
- Add the NuGet Package `Hl7.Fhir.R4` (latest version is 3.3.0) to your C# project
- In the main code, create a static object of type `Patient` and fill in some properties of this patient, for example a `Name`, `Active` and `Birthday`:
```c#
        private static Patient _patient = new()
        {   
            Name = new List<HumanName> { new HumanName { Family = "Visser" } },
            Active = true,
            BirthDate = "2001-03-01",
        };
```
- Let's serialize this patient to a string so we can display it in our console. For that we can use the `JsonSerializer`. When don't want to have to whole json on 1 line, you can use the `JsonSerializerOptions` and set `WriteIndented` to true.
```c#
    // pretty print the json
    var options = new JsonSerializerOptions(){WriteIndented = true}.ForFhir(typeof(Patient).Assembly);
    var json = JsonSerializer.Serialize<Patient>(patFromServer, options);
```

- So we have our (in memory) patient ready. Now let's validate this patient to make sure it meets the FHIR rules.

- Add the NuGet Package `Hl7.Fhir.Specification.R4` (latest version is 4.0.0) to your C# project  
-	Add to your main code, the following using statement to include the validator:
```c#
    using Hl7.Fhir.Validation;
    using Hl7.Fhir.Specification.Source;
```
-	Create a new Validator instance:
```c#
    var validator = new Validator();
```
-	Try to validate a single patient:
```c#
    var outcome = validator.Validate(_patient);
```

- The outcome of the validator is of type `OperationOutcome` and is also a FHIR resource. We can serialize this to a string and write this to the console:
```c#
    // print the outcome
    Console.WriteLine($"Success: {outcome.Success} \n{JsonSerializer.Serialize<Bundle>(results, options)}");
```
- Now, run the console application and check the outcome of the validation operation. This outcome has some properties that you can use:
  - Success: a Boolean which indicates whether the validation was successful or not
  - Issue: a list of issues that were raised during validation
  -  See [this link](https://www.hl7.org/fhir/operationoutcome.html) for more information about the OperationOutcome.

-	You will notice that the validation fails. The message `[ERROR] Unable to resolve reference to profile 'http://hl7.org/fhir/StructureDefinition/Patient'` is shown.
The validator needs the standard Patient profile (StructureDefinition) to validate the instance. So, we must tell the validator where to find this this profile. We do this by passing a FhirPackageSource to the validator. For all the standard HL7 FHIR resources, the SDK has a special ResourceResolver already made for you:
```c#
    var resolver = new CachedResolver(new FhirPackageSource(
                "https://packages.simplifier.net",
                new[] { "hl7.fhir.r4.core" }
            ));

    settings.ResourceResolver = new CachedResolver(resolver);
    var validator = new Validator(settings);
```
- Note that we wrap the standard ResourceResolver in a `CachedResolver`. This will speed up the validation when you validate more than 1 resource.

-	Run the program again and you will see that the validation of Patient is successful.
- The field `language` in `Communication` is mandatory (see also https://www.hl7.org/fhir/patient.html). When we add a communcation item to patient and leave out the language, the validator should report this. Try this out.

The validator can also use other profiles to validate against. For example the profile `Us-core-patient`, see [here](http://hl7.org/fhir/us/core/STU3.1.1/StructureDefinition-us-core-patient.html) for the definition.
In the next steps we are going to validate our in memory patient to this us-core-patient profile.

- In order to use these profiles we have to tell the validator where to find this profile. We do this with a `DirectorySource`:
```c#
    var resolver = new CachedResolver(new FhirPackageSource(
                "https://packages.simplifier.net",
                new[] { "hl7.fhir.us.core@3.1.1" }
            ));
```

-	Let’s validate our patient against this new us-core profile:
```c#
    var outcome = validator.Validate(obs, new[] {
               "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient" });
```
- You will see that the validation fails, because an identifier and gender is mandatory.
- Change your in-memory patient so that it validates again.
- You can also add the profile in the meta part of the patient. The validator will pick that up and uses this profile to validate your instance. Try this out.

## Session 2b – Communicate with a FHIR server
**Exercise**: For this exercise, we will be using the in-memory patient from session 2a and send this FHIR resource to a
FHIR server. In addition, we will cover a few search methods to find resources on a FHIR server.

### Exercise steps
- Open your previously created app (session 2a)
- To your main code, add a using statement to include the FHIR RESTful client:
```
using Hl7.Fhir.Rest;
```
- Create a new FhirClient object, pointing it to the public Firely test server "https://server.fire.ly/r4". With the FhirClient, you can use methods for the RESTful interactions:
```c#
var client = new FhirClient("https://server.fire.ly/r4");
```
- Let's see what the Firely Server is capable of. We can do this by retrieving the capability statement of the server:
```c#
 var capability = client.CapabilityStatement();
```
- This object is also a FHIR Resource (https://www.hl7.org/fhir/capabilityStatement.html) and can be serialized to a string. For now we are only interested in a few properties, like the name and `FhirVersion`. Let's print those to the console:
```c#
   Console.WriteLine($"capability: Name: {capability.Name}, Fhir Version: {capability.FhirVersion} ");
```
- It should return the following:
```
capability: Name: Firely Server 4.2.0 CapabilityStatement, Fhir Version: N4_0_1
```
- So we made sure that our FHIR server can handle FHIR R4 (4.0.1) resources. Now let's try to validate our in-memory patient on the FHIR server. Actually the same procedure what we have done in session 2a, only now the validation is done by the server. To accomplish this, we use the method `ValidateResource` of the FhirClient. This function will upload the resource to the server and the server will validate this resource and gives back an OperationOutcome to the client:
```c#
var oo = client.ValidateResource(_patient);
```
- You will notice that the resource is not validate, because we still have the profile of us-core-patient in the meta section of our patient, and this profile does not exist on the FHIR server, only the standard FHIR profiles.
- Remove the meta section in our patient instance and try again.
- Now let's try to upload our in-memory patient and persist it on our FHIR server. We will use the method `Create` for that:
```c#
var pat = client.Create<Patient>(_patient);
```
- If you use the `Create` method, the server will assign a new technical ID to the resource. The return value of the function Create will have this patient and technical ID. You can write this id to the console.
- Using the Update method, your Patient will be created with the technical ID you have assigned, or updated if a Patient with that technical ID already exists – please note that a production server will not always allow this.

- We can now try to read this patient back from the server. We use the function `Read<Patient>` for that:
```c#
var patFromServer = client.Read<Patient>($"Patient/{pat.Id}");
```
- Note that the meta section now also contains `versionId` and `lastUpdated` fields.

- The FHIR client has several operations to do basic search. For example to search for all patient with family Name = "Visser" we can do the following:
```c#
Bundle results = client.Search<Patient>(new string[] { "family:exact=Visser" });
```

- For more complex searches you can use the Firely .NET SDK SearchParams class. Like so:
```c#
// using the class SearchParams
var q = new SearchParams()
    .Where("family:exact=Visser")
    .OrderBy("birthdate", SortOrder.Descending)
    .SummaryOnly()
    .Include("Patient:organization")
    .LimitTo(5);
results = client.Search<Patient>(q);
```
- This will search for patients with an exact family name = 'Visser', ordered by birthdate and include organization (if present). Only the summary is returned (so not all fields). And also limit the results to 5 patients.

- The example above limits the results to only 5 patient. To retrieve the 5 next patients, just use the FhirClient function `Continue`:
```c#
// continue with the next page:
results = client.Continue(results);
```

Have fun, and remember to ask for help if you get stuck!

## Further information
Some useful links:
- Extra documentation for FhirClient: https://docs.fire.ly/projects/Firely-NET-SDK/client.html
- HL7 Fhir Restful API specification: https://www.hl7.org/fhir/http.html
- Zulip stream for asking questions about Firely .NET SDK: https://chat.fhir.org/#narrow/stream/179171-dotnet
- List of Fhir Test servers: https://confluence.hl7.org/display/FHIR/Public+Test+Servers
- SMART overview: https://github.com/GinoCanessa/FhirDevVideoNotes/tree/main/03-Getting-SMART
