using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using RockLib.Configuration;
using RockLib.Immutable;
using Xunit;

namespace RockLib.Serialization.Tests
{
    public class SerializerTests
    {
        private static readonly FieldInfo _jsonSerializersField =
            typeof(Serializer).GetField("_jsonSerializers", BindingFlags.NonPublic | BindingFlags.Static)!;
        private static readonly FieldInfo _xmlSerializersField =
            typeof(Serializer).GetField("_xmlSerializers", BindingFlags.NonPublic | BindingFlags.Static)!;

        [Fact]
        public void DefaultJsonSerializersIsUsedWhenThereIsNoConfig()
        {
            ResetSeralizers();
            ResetConfig();

            Config.SetRoot(new ConfigurationBuilder().Build());
            Serializer.SetJsonSerializers();

            var jsonSerializer = (DefaultJsonSerializer)Serializer.JsonSerializers?.First()!;

            jsonSerializer.Should().NotBeNull();
            jsonSerializer.Name.Should().Be("default");
            jsonSerializer.JsonSerializer.Formatting.Should().Be(Formatting.None);
        }

        [Fact]
        public void DefaultXmlSerializerIsUsedWhenThereIsNoConfig()
        {
            ResetSeralizers();
            ResetConfig();

            Config.SetRoot(new ConfigurationBuilder().Build());
            Serializer.SetXmlSerializers();

            var xmlSerializer = (DefaultXmlSerializer)Serializer.XmlSerializers?.First()!;

            xmlSerializer.Should().NotBeNull();
            xmlSerializer.Name.Should().Be("default");
            xmlSerializer.WriterSettings.Should().BeNull();
        }

        [Fact]
        public void ConfigSettingsAreUsedWhenNullIsPassedIntoSetJsonSerializers()
        {
            ResetSeralizers();
            ResetConfig();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["rocklib.serialization:JsonSerializers:Settings:Formatting"] = "Indented",
                })
                .Build();

            Config.SetRoot(config);
            Serializer.SetJsonSerializers();

            var jsonSerializer = (DefaultJsonSerializer)Serializer.JsonSerializers?.First()!;

            jsonSerializer.Should().NotBeNull();
            jsonSerializer.Name.Should().Be("default");
            jsonSerializer.JsonSerializer.Formatting.Should().Be(Formatting.Indented);
        }

        [Fact]
        public void ConfigSettingsAreUsedWhenNullIsPassedIntoSetXmlSerializers()
        {
            ResetSeralizers();
            ResetConfig();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["rocklib.serialization:XmlSerializers:WriterSettings:Indent"] = "false",
                })
                .Build();

            Config.SetRoot(config);
            Serializer.SetXmlSerializers();

            var xmlSerializer = (DefaultXmlSerializer)Serializer.XmlSerializers?.First()!;

            xmlSerializer.Should().NotBeNull();
            xmlSerializer.Name.Should().Be("default");
            xmlSerializer.WriterSettings!.Indent.Should().BeFalse();
        }

        [Fact]
        public void SetJsonSerializersMethodsSetTheSerializers()
        {
            ResetSeralizers();

            var jsonSerializerMock = new Mock<ISerializer>();

            jsonSerializerMock.Setup(m => m.Name).Returns("default");

            Serializer.SetJsonSerializers(new List<ISerializer> { jsonSerializerMock.Object });

            Serializer.JsonSerializers.First().Should().Be(jsonSerializerMock.Object);
        }

        [Fact]
        public void SetXmlSerializersMethodsSetTheSerializers()
        {
            ResetSeralizers();

            var xmlSerializerMock = new Mock<ISerializer>();

            xmlSerializerMock.Setup(m => m.Name).Returns("default");

            Serializer.SetXmlSerializers(new List<ISerializer> { xmlSerializerMock.Object });

            Serializer.XmlSerializers.First().Should().Be(xmlSerializerMock.Object);
        }

        [Fact]
        public void ToJson1CallsSerializeToStringWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            var toSerialize = new Type1ForSerializer();
            var json = toSerialize.ToJson();

            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToJson1CallsSerializeToStringWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            var toSerialize = new Type1ForSerializer();
            var json = toSerialize.ToJson("notdefault");

            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToJson2CallsSerializeToStringWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            var toSerialize = new Type1ForSerializer();
            var json = toSerialize.ToJson(typeof(Type2ForSerializer));

            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToJson2CallsSerializeToStringWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            var toSerialize = new Type1ForSerializer();
            var json = toSerialize.ToJson(typeof(Type2ForSerializer), "notdefault");

            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Once);
        }

        [Fact]
        public void ToJson3CallsSerializeToStreamWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            using var stream = new MemoryStream();
            var toSerialize = new Type1ForSerializer();
            toSerialize.ToJson(stream);

            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToJson3CallsSerializeToStreamWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            using var stream = new MemoryStream();
            var toSerialize = new Type1ForSerializer();
            toSerialize.ToJson(stream, "notdefault");

            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToJson4CallsSerializeToStreamWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            using var stream = new MemoryStream();
            var toSerialize = new Type1ForSerializer();
            toSerialize.ToJson(typeof(Type2ForSerializer), stream);

            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToJson4CallsSerializeToStreamWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            using var stream = new MemoryStream();
            var toSerialize = new Type1ForSerializer();
            toSerialize.ToJson(typeof(Type2ForSerializer), stream, "notdefault");

            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Once);
        }

        [Fact]
        public void ToXml1CallsSerializeToStringWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            var toSerialize = new Type1ForSerializer();
            var xml = toSerialize.ToXml();

            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToXml1CallsSerializeToStringWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            var toSerialize = new Type1ForSerializer();
            var xml = toSerialize.ToXml("notdefault");

            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToXml2CallsSerializeToStringWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            var toSerialize = new Type1ForSerializer();
            var xml = toSerialize.ToXml(typeof(Type2ForSerializer));

            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToXml2CallsSerializeToStringWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            var toSerialize = new Type1ForSerializer();
            var xml = toSerialize.ToXml(typeof(Type2ForSerializer), "notdefault");

            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Once);
        }

        [Fact]
        public void ToXml3CallsSerializeToStreamWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            using var stream = new MemoryStream();
            var toSerialize = new Type1ForSerializer();
            toSerialize.ToXml(stream);

            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToXml3CallsSerializeToStreamWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            using var stream = new MemoryStream();
            var toSerialize = new Type1ForSerializer();
            toSerialize.ToXml(stream, "notdefault");

            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToXml4CallsSerializeToStreamWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            using var stream = new MemoryStream();
            var toSerialize = new Type1ForSerializer();
            toSerialize.ToXml(typeof(Type2ForSerializer), stream);

            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void ToXml4CallsSerializeToStreamWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            using var stream = new MemoryStream();
            var toSerialize = new Type1ForSerializer();
            toSerialize.ToXml(typeof(Type2ForSerializer), stream, "notdefault");

            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.SerializeToStream(stream, It.IsAny<object>(), typeof(Type2ForSerializer)), Times.Once);
        }

        [Fact]
        public void FromJson1CallsDeserializeFromStringWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            var deserialized = "{}".FromJson<Type1ForSerializer>();

            deserialized.Should().BeOfType<Type1ForSerializer>();
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void FromJson1CallsDeserializeFromStringWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            var deserialized = "{}".FromJson<Type1ForSerializer>("notdefault");

            deserialized.Should().BeOfType<Type1ForSerializer>();
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void FromJson2CallsDeserializeFromStringWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            var deserialized = "{}".FromJson(typeof(Type2ForSerializer));

            deserialized.Should().BeOfType<Type2ForSerializer>();
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void FromJson2CallsDeserializeFromStringWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetJsonSerializers(serializerMocks.Values.Select(v => v.Object));

            var deserialized = "{}".FromJson(typeof(Type2ForSerializer), "notdefault");

            deserialized.Should().BeOfType<Type2ForSerializer>();
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Once);
        }

        [Fact]
        public void FromXml1CallsDeserializeFromStringWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            var deserialized = "{}".FromXml<Type1ForSerializer>();

            deserialized.Should().BeOfType<Type1ForSerializer>();
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void FromXml1CallsDeserializeFromStringWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            var deserialized = "{}".FromXml<Type1ForSerializer>("notdefault");

            deserialized.Should().BeOfType<Type1ForSerializer>();
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void FromXml2CallsDeserializeFromStringWithDefaultName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            var deserialized = "{}".FromXml(typeof(Type2ForSerializer));

            deserialized.Should().BeOfType<Type2ForSerializer>();
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Once);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
        }

        [Fact]
        public void FromXml2CallsDeserializeFromStringWithName()
        {
            ResetSeralizers();

            var serializerMocks = GetSerializerMocks();
            Serializer.SetXmlSerializers(serializerMocks.Values.Select(v => v.Object));

            var deserialized = "{}".FromXml(typeof(Type2ForSerializer), "notdefault");

            deserialized.Should().BeOfType<Type2ForSerializer>();
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["default"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer)), Times.Never);
            serializerMocks["notdefault"].Verify(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer)), Times.Once);
        }

        private static Dictionary<string, Mock<ISerializer>> GetSerializerMocks(Stream? stream = null)
        {
            var serializers = new Dictionary<string, Mock<ISerializer>>();

            var serializerMock1 = new Mock<ISerializer>();
            serializerMock1.Setup(m => m.Name).Returns("default");
            serializerMock1.Setup(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer))).Returns("");
            serializerMock1.Setup(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer))).Returns("");
            serializerMock1.Setup(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer))).Returns(new Type1ForSerializer());
            serializerMock1.Setup(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer))).Returns(new Type2ForSerializer());
            serializerMock1.Setup(m => m.SerializeToStream(stream!, It.IsAny<object>(), typeof(Type1ForSerializer)));
            serializerMock1.Setup(m => m.SerializeToStream(stream!, It.IsAny<object>(), typeof(Type2ForSerializer)));
            serializerMock1.Setup(m => m.DeserializeFromStream(stream!, typeof(Type1ForSerializer))).Returns(new Type1ForSerializer());
            serializerMock1.Setup(m => m.DeserializeFromStream(stream!, typeof(Type2ForSerializer))).Returns(new Type2ForSerializer());

            var serializerMock2 = new Mock<ISerializer>();
            serializerMock2.Setup(m => m.Name).Returns("notdefault");
            serializerMock2.Setup(m => m.SerializeToString(It.IsAny<object>(), typeof(Type1ForSerializer))).Returns("");
            serializerMock2.Setup(m => m.SerializeToString(It.IsAny<object>(), typeof(Type2ForSerializer))).Returns("");
            serializerMock2.Setup(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type1ForSerializer))).Returns(new Type1ForSerializer());
            serializerMock2.Setup(m => m.DeserializeFromString(It.IsAny<string>(), typeof(Type2ForSerializer))).Returns(new Type2ForSerializer());
            serializerMock2.Setup(m => m.SerializeToStream(stream!, It.IsAny<object>(), typeof(Type1ForSerializer)));
            serializerMock2.Setup(m => m.SerializeToStream(stream!, It.IsAny<object>(), typeof(Type2ForSerializer)));
            serializerMock2.Setup(m => m.DeserializeFromStream(stream!, typeof(Type1ForSerializer))).Returns(new Type1ForSerializer());
            serializerMock2.Setup(m => m.DeserializeFromStream(stream!, typeof(Type2ForSerializer))).Returns(new Type2ForSerializer());

            serializers.Add(serializerMock1.Object.Name, serializerMock1);
            serializers.Add(serializerMock2.Object.Name, serializerMock2);

            return serializers;
        }

        private static void ResetSeralizers()
        {
            var jsonSerializers = (Semimutable<Dictionary<string, ISerializer>>)_jsonSerializersField.GetValue(null)!;
            jsonSerializers.GetUnlockValueMethod().Invoke(jsonSerializers, null);

            var xmlSerializers = (Semimutable<Dictionary<string, ISerializer>>)_xmlSerializersField.GetValue(null)!;
            xmlSerializers.GetUnlockValueMethod().Invoke(xmlSerializers, null);
        }

        private static void ResetConfig()
        {
            var rootField = typeof(Config).GetField("_root", BindingFlags.NonPublic | BindingFlags.Static)!;

            var root = (Semimutable<IConfiguration>)rootField.GetValue(null)!;
            root.GetUnlockValueMethod().Invoke(root, null);
            root.ResetValue();
        }

        private class Type1ForSerializer
        {
        }

        private class Type2ForSerializer
        {
        }
    }
}
