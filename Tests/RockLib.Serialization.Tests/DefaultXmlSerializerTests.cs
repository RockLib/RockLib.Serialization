using System;
using System.IO;
using System.Text;
using System.Xml;
using FluentAssertions;
using Xunit;

namespace RockLib.Serialization.Tests;

public static class DefaultXmlSerializerTests
{
    private static readonly TypeForXmlSerializer _expectedItem = new() { PropA = 5, PropB = true, PropC = "PropC" };
    private const string EXPECTED_XML = @"<?xml version=""1.0"" encoding=""utf-8""?><TypeForXmlSerializer xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><PropA>5</PropA><PropB>true</PropB><PropC>PropC</PropC></TypeForXmlSerializer>";

    [Fact]
    public static void EmptyConstructorCreatesDefaultValues()
    {
        var serializer = new DefaultXmlSerializer();

        serializer.Name.Should().Be("default");
        serializer.WriterSettings.Should().BeNull();
        serializer.ReaderSettings.Should().BeNull();
        serializer.Namespaces.Should().BeNull();
    }

    [Fact]
    public static void ConstructorWithNullNameCreatesDefaultName()
    {
        var serializer = new DefaultXmlSerializer(null!);

        serializer.Name.Should().Be("default");
    }

    [Fact]
    public static void ConstructorPassesValuesCorrectly()
    {
        var name = "notdefault";
        var xmlWriterSettings = new XmlWriterSettings();
        var xmlReaderSettings = new XmlReaderSettings();
        var nameSpaces = new[] { new XmlQualifiedName("Name1", "Namespace1"), new XmlQualifiedName("Name2", "Namespace2") };

        var serializer = new DefaultXmlSerializer(name, nameSpaces, xmlWriterSettings, xmlReaderSettings);

        serializer.Name.Should().Be(name);
        serializer.ReaderSettings.Should().BeSameAs(xmlReaderSettings);
        serializer.WriterSettings.Should().BeSameAs(xmlWriterSettings);
        serializer.Namespaces!.ToArray().Should().BeEquivalentTo(nameSpaces);
    }

    [Fact]
    public static void SerializeToStreamThrowWhenStreamIsNull()
    {
        Action action = () => new DefaultXmlSerializer().SerializeToStream(null!, new object(), typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenItemIsNull()
    {
        Action action = () => new DefaultXmlSerializer().SerializeToStream(new MemoryStream(), null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenTypeIsNull()
    {
        Action action = () => new DefaultXmlSerializer().SerializeToStream(new MemoryStream(), new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenStreamIsNull()
    {
        Action action = () => new DefaultXmlSerializer().DeserializeFromStream(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenTypeIsNull()
    {
        Action action = () => new DefaultXmlSerializer().DeserializeFromStream(new MemoryStream(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenItemIsNull()
    {
        Action action = () => new DefaultXmlSerializer().SerializeToString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenTypeIsNull()
    {
        Action action = () => new DefaultXmlSerializer().SerializeToString(new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenItemIsNull()
    {
        Action action = () => new DefaultXmlSerializer().DeserializeFromString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*data*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenTypeIsNull()
    {
        Action action = () => new DefaultXmlSerializer().DeserializeFromString("", null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStreamSerializesCorrectly()
    {
        var serializer = new DefaultXmlSerializer(writerSettings: new XmlWriterSettings { Encoding = Encoding.UTF8 });

        using var stream = new MemoryStream();
        serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForXmlSerializer));

        using var streamReader = new StreamReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        var xml = streamReader.ReadToEnd();
        xml.Should().Be(EXPECTED_XML);
    }

    [Fact]
    public static void DeserializeFromStreamDeserializesCorrectly()
    {
        var serializer = new DefaultXmlSerializer();

        TypeForXmlSerializer item;
        using (var stream = new MemoryStream())
        {
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
            {
                writer.Write(EXPECTED_XML);
            }
            stream.Seek(0, SeekOrigin.Begin);

            item = (TypeForXmlSerializer)serializer.DeserializeFromStream(stream, typeof(TypeForXmlSerializer))!;
        }

        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(_expectedItem);
    }

    [Fact]
    public static void SerializeToStringSerializesCorrectly()
    {
        var serializer = new DefaultXmlSerializer(writerSettings: new XmlWriterSettings());

        var xml = serializer.SerializeToString(_expectedItem, typeof(TypeForXmlSerializer));

        xml.Should().Be(EXPECTED_XML);
    }

    [Fact]
    public static void DeserializeFromStringDeserializesCorrectly()
    {
        var serializer = new DefaultXmlSerializer();

        var item = serializer.DeserializeFromString(EXPECTED_XML, typeof(TypeForXmlSerializer)) as TypeForXmlSerializer;

        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(_expectedItem);
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public class TypeForXmlSerializer
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public int PropA { get; set; }
        public bool PropB { get; set; }
        public string? PropC { get; set; }
    }
}
