---
sidebar_position: 3
---

# How to configure and use named serializers

If an application needs to use different serializers depending on the scenario, then _named serializers_ are used. By default, serializers are named "default", so named serializers should given any name other than "default". To serialize or deserialize with a named serializer, pass a value for the optional `name` parameter in the `ToJson`, `FromJson`, `ToXml`, or `FromXml` extension methods.

## Configuring named serializers

Configuring named serializers is as simple as providing a unique name for the serializer. It can be done programmatically...

```csharp
ISerializer defaultJsonSerializer = new DefaultJsonSerializer();
ISerializer myJsonSerializer = new MyJsonSerializer(name: "CustomSerializer", firstParam: 123, secondParam: "abc");

Serializer.SetJsonSerializers(new[] { defaultJsonSerializer, myJsonSerializer });
```

With the static `Config` class. The previous programmatic example is equivalent to the following appsetting.json example:

```json
{
  "RockLib.Serialization": {
    "JsonSerializers": [
      { "Type": "RockLib.Serialization.DefaultJsonSerializer, RockLib.Serialization" },
      {
        "Type": "MyNamespace.MyJsonSerializer, MyAssembly",
        "Value": {
          "Name": "CustomSerializer"
          "FirstParam": 123,
          "SecondParam": "abc"
        }
      }
    ]
  }
}
```

## Using named serializers

To use named serializers the `ToJson`, `FromJson`, `ToXml`, or `FromXml` extension methods, provide a value for the optional `name` parameter.

```csharp
Company company = new Company { Name = "Company, Inc." };
Person person = new Person { FirstName = "Joe", LastName = "Public" };

// Serialize and deserialize company with the default serializer.
string companyJson = company.ToJson();
Company companyFromJSon = companyJson.FromJson<Company>();

// Serialize and deserialize person with the serializer named "CustomSerializer".
string personJson = person.ToJson("CustomSerializer");
Person personFromJson = personJson.FromJson<Person>("CustomSerializer");
```
