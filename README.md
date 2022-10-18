# RockLib.Serialization

*A simple serialization abstraction with To and From extensions.*

## Packages

### RockLib.Serialization

The main library. Contains `ISerializer` interface, `DefaultJsonSerializer` and `DefaultXmlSerializer` implementations, and `ToJson`, `FromJson`, `ToXml`, and `FromXml` extension methods.

### RockLib.Serialization.XSerializer

Contains the `XSerializerJsonSerializer` and `XSerializerXmlSerializer` implementations of the `ISerializer` interface.

### RockLib.Serialization.DataContract

Contains the `DataContractJsonSerializer` and `DataContractXmlSerializer` implementations of the `ISerializer` interface.

---

- [Getting started](docs/GettingStarted.md)
- How to:
  - [Configure serializers](docs/ConfigureSerializers.md)
  - [Configure and use named serializers](docs/NamedSerializers.md)
