# RockLib.Serialization

*A simple serialization abstraction with To and From extensions.*

### RockLib.Serialization [![Build status](https://ci.appveyor.com/api/projects/status/5q2c272sgo8dl6p2?svg=true)](https://ci.appveyor.com/project/RockLib/rocklib-serialization) [![NuGet](https://img.shields.io/nuget/vpre/RockLib.Serialization.svg)](https://www.nuget.org/packages/RockLib.Serialization)

The main library. Contains `ISerializer` interface, `DefaultJsonSerializer` and `DefaultXmlSerializer` implementations, and `ToJson`, `FromJson`, `ToXml`, and `FromXml` extension methods.

### RockLib.Serialization.XSerializer [![Build status](https://ci.appveyor.com/api/projects/status/hv1q0g024iepn6j0?svg=true)](https://ci.appveyor.com/project/RockLib/rocklib-serialization-v3nkb) [![NuGet](https://img.shields.io/nuget/vpre/RockLib.Serialization.XSerializer.svg)](https://www.nuget.org/packages/RockLib.Serialization.XSerializer)

Contains the `XSerializerJsonSerializer` and `XSerializerXmlSerializer` implementations of the `ISerializer` interface.

### RockLib.Serialization.DataContract [![Build status](https://ci.appveyor.com/api/projects/status/xwhs4snx62enst7q?svg=true)](https://ci.appveyor.com/project/RockLib/rocklib-serialization-80kpe) [![NuGet](https://img.shields.io/nuget/vpre/RockLib.Serialization.DataContract.svg)](https://www.nuget.org/packages/RockLib.Serialization.DataContract)

Contains the `DataContractJsonSerializer` and `DataContractXmlSerializer` implementations of the `ISerializer` interface.

---

- [Getting started](docs/GettingStarted.md)
- How to:
  - [Configure serializers](docs/ConfigureSerializers.md)
  - [Configure and use named serializers](docs/NamedSerialiers.md)
