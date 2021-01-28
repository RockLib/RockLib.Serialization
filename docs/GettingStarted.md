# Getting Started

In this tutorial, we will be building a console application that writes a serialized `Person` object to console as JSON and XML. It then deserializes the JSON and XML back into `Person` objects and verifies that their values are the same.

---

Create a .NET Core console application named "SerializationTutorial".

---

Add a nuget reference for "RockLib.Serialization".

---

Add a new class named 'Person' to the project:

```c#
namespace SerializationTutorial
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
```

---

Edit the `Program.cs` file as follows:

```c#
using RockLib.Serialization;

namespace SerializationTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            Person person = new Person();

            Console.Write("FirstName>");
            person.FirstName = Console.ReadLine();
            Console.Write("LastName>");
            person.LastName = Console.ReadLine();
            Console.WriteLine();

            string jsonString = person.ToJson();
            string xmlString = person.ToXml();

            Console.WriteLine("JSON:");
            Console.WriteLine(jsonString);
            Console.WriteLine();
            Console.WriteLine("XML:");
            Console.WriteLine(xmlString);
            Console.WriteLine();

            Person jsonPerson = jsonString.FromJson<Person>();
            Person xmlPerson = xmlString.FromXml<Person>();

            Console.WriteLine($"jsonPerson.FirstName == xmlPerson.FirstName: {jsonPerson.FirstName == xmlPerson.FirstName}");
            Console.WriteLine($"jsonPerson.LastName == xmlPerson.LastName: {jsonPerson.LastName == xmlPerson.LastName}");
        }
    }
}
```

---

Start the app and enter a first and last name. It should output something like this:

```
FirstName>Joe
LastName>Public

JSON:
{"FirstName":"Joe","LastName":"Public"}

XML:
<?xml version="1.0" encoding="utf-8"?>
<Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <FirstName>Joe</FirstName>
  <LastName>Public</LastName>
</Person>

jsonPerson.FirstName == xmlPerson.FirstName: True
jsonPerson.LastName == xmlPerson.LastName: True
```
