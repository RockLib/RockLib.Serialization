# **RockLib.Serialization Deprecation**

RockLib has been a cornerstone of our open source efforts here at Rocket Mortgage, and it's played a significant role in our journey to drive innovation and collaboration within our organization and the open source community. It's been amazing to witness the collective creativity and hard work that you all have poured into this project.

However, as technology relentlessly evolves, so must we. The decision to deprecate this library is rooted in our commitment to staying at the cutting edge of technological advancements. While this chapter is ending, it opens the door to exciting new opportunities on the horizon.

We want to express our heartfelt thanks to all the contributors and users who have been a part of this incredible journey. Your contributions, feedback, and enthusiasm have been invaluable, and we are genuinely grateful for your dedication. 🚀

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
