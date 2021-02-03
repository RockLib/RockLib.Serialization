# How to configure serializers

By default, JSON serialization is done by an instance of `DefaultJsonSerializer` and XML serialization is done by an instance of `DefaultXmlSerializer`, where each is constructed with no optional parameters provided. [See below](#default-serializers) for details on these serializers.

There are two ways of setting custom JSON and XML serializers: programmatically and through the `Config` static class (from the RockLib.Configuration package).

Programmatically:

```c#
// Assuming the MyJsonSerializer and MyXmlSerializer classes exist...
ISerializer jsonSerializer = new MyJsonSerializer(firstParam: 123, secondParam: "abc");
ISerializer xmlSerializer = new MyXmlSerializer(firstParam: 456, secondParam: "xyz");

Serializer.SetJsonSerializers(new[] { jsonSerializer });
Serializer.SetXmlSerializers(new[] { xmlSerializer });
```

Through the `Config` static class via appsettings.json:

```json
{
  "RockLib.Serialization": {
    "JsonSerializers": {
      "Type": "MyNamespace.MyJsonSerializer, MyAssembly",
      "Value": {
        "FirstParam": 123,
        "SecondParam": "abc"
      }
    },
    "XmlSerializers": {
      "Type": "MyNamespace.MyXmlSerializer, MyAssembly",
      "Value": {
        "FirstParam": 456,
        "SecondParam": "xyz"
      }
    } 
  } 
}
```

## Default serializers

The main RockLib.Serialization package contains the two default implementations of the `ISerializer` interface: `DefaultJsonSerializer` and `DefaultXmlSerializer`. All of the constructor parameters in both classes are optional.

```json
{
  "RockLib.Serialization": {
    "JsonSerializers": {
      "Settings": {
        "Formatting": "Indented"
      }
    },
    "XmlSerializers": {
      "Namespaces": [
        { "name" : "prefix1", "ns" : "namespace1" },
        { "name" : "prefix2", "ns" : "namespace2" },
        { "name" : "prefix3", "ns" : "namespace3" }
      ],
      "WriterSettings" : {
        "Indent": false
      }
    } 
  } 
}
```

#### DefaultJsonSerializer

| Parameter type | Parameter name | Description |
|:--|:--|:--|
| `string` | name | The name of the serializer, used to when selecting which serializer to use. |
| `JsonSerializerSettings` | settings | The Newtonsoft.Json settings used for the serialization. |

#### DefaultXmlSerializer

| Parameter type | Parameter name | Description |
|:--|:--|:--|
| `string` | name | The name of the serializer, used to when selecting which serializer to use. |
| `XmlQualifiedName[]` | namespaces | The objects that define the namespace prefixes for serialization. |
| `XmlWriterSettings` | writerSettings | The object that defines the settings for the `XmlWriter`. |
| `XmlReaderSettings` | readerSettings | The object that defines the settings for the `XmlReader`. |

## XSerializer serializers

The RockLib.Serialization.XSerializer package contains two implementations of the `ISerializer` interface: `XSerializerJsonSerializer` and `XSerializerXmlSerializer`. All of the constructor parameters in both classes are optional.

```json
{
  "RockLib.Serialization": {
    "JsonSerializers": {
      "Type": "RockLib.Serialization.XSerializer.XSerializerJsonSerializer, RockLib.Serialization.XSerializer",
      "Value": {
        "Options": {
          "Encoding": { "Type": "System.Text.UTF32Encoding, System.Text.Encoding.Extensions" }
        }
      }
    },
    "XmlSerializers": {
      "Type": "RockLib.Serialization.XSerializer.XSerializerXmlSerializer, RockLib.Serialization.XSerializer",
      "Value": {
        "Options": {
          "Encoding": { "Type": "System.Text.UTF32Encoding, System.Text.Encoding.Extensions" }
        }
      }
    }
  }
}
```

#### XSerializerJsonSerializer

| Parameter type | Parameter name | Description |
|:--|:--|:--|
| `string` | name | The name of the serializer, used to when selecting which serializer to use. |
| `JsonSerializerConfiguration` | options | Options for customizing the JsonSerializer. |


#### XSerializerXmlSerializer

| Parameter type | Parameter name | Description |
|:--|:--|:--|
| `string` | name | The name of the serializer, used to when selecting which serializer to use. |
| `XmlSerializationOptions` | options | Options for customizing the XmlSerializer. |


## Data Contract serializers

The RockLib.Serialization.DataContract package contains two implementations of the `ISerializer` interface: `DataContractJsonSerializer` and `DataContractXmlSerializer`. All of the constructor parameters in both classes are optional.

```json
{
  "RockLib.Serialization": {
    "JsonSerializers": {
      "Type": "RockLib.Serialization.DataContract.DataContractJsonSerializer, RockLib.Serialization.DataContract",
      "Value": {
        "Settings": {
          "SerializeReadOnlyTypes": true,
          "DateTimeFormat": { "FormatString" : "O" }
        }
      }
    },
    "XmlSerializers": {
      "Type": "RockLib.Serialization.DataContract.DataContractXmlSerializer, RockLib.Serialization.DataContract",
      "Value": {
        "Settings": {
          "SerializeReadOnlyTypes": true
        }
      }
    }
  }
}
```

#### DataContractJsonSerializer

| Parameter type | Parameter name | Description |
|:--|:--|:--|
| `string` | name | The name of the serializer, used to when selecting which serializer to use. |
| `DataContractJsonSerializerSettings` | settings | Options for customizing the JsonSerializer. |


#### DataContractXmlSerializer

| Parameter type | Parameter name | Description |
|:--|:--|:--|
| `string` | name | The name of the serializer, used to when selecting which serializer to use. |
| `DataContractSerializerSettings` | settings | Options for customizing the XmlSerializer. |
