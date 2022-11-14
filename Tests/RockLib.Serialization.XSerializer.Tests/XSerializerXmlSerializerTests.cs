using System;
using System.Globalization;
using System.IO;
using System.Text;
using FluentAssertions;
using XSerializer;
using Xunit;

namespace RockLib.Serialization.XSerializer.Tests;

public static class XSerializerXmlSerializerTests
{
    private static readonly TypeForXmlSerializer _expectedItem = new() { PropA = 5, PropB = true, PropC = "PropC" };
    private const string STREAM_HEADER = @"<?xml version=""1.0"" encoding=""utf-8""?>";
    private const string STRING_HEADER = @"<?xml version=""1.0"" encoding=""utf-16""?>";
    private const string EXPECTED_XML_FORMAT = @"{0}<TypeForXmlSerializer xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><PropA>5</PropA><PropB>true</PropB><PropC>PropC</PropC></TypeForXmlSerializer>";

    [Fact]
    public static void EmptyConstructorCreatesDefaultValues()
    {
        var serializer = new XSerializerXmlSerializer();

        serializer.Name.Should().Be("default");
        serializer.Options.Should().BeNull();
    }

    [Fact]
    public static void ConstructorWithNullNameCreatesDefaultName()
    {
        var serializer = new XSerializerXmlSerializer(null!);

        serializer.Name.Should().Be("default");
    }

    [Fact]
    public static void ConstructorPassesValuesCorrectly()
    {
        var name = "notdefault";
        var options = new XmlSerializationOptions(encoding: Encoding.UTF32);
        var serializer = new XSerializerXmlSerializer(name, options);

        serializer.Name.Should().Be(name);
        serializer.Options.Should().NotBeNull();
        serializer.Options.Should().BeSameAs(options);
    }

    [Fact]
    public static void SerializeToStreamThrowWhenStreamIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().SerializeToStream(null!, new object(), typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenItemIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().SerializeToStream(new MemoryStream(), null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenTypeIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().SerializeToStream(new MemoryStream(), new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenStreamIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().DeserializeFromStream(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenTypeIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().DeserializeFromStream(new MemoryStream(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenItemIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().SerializeToString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenTypeIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().SerializeToString(new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenItemIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().DeserializeFromString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*data*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenTypeIsNull()
    {
        Action action = () => new XSerializerXmlSerializer().DeserializeFromString("", null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStreamSerializesCorrectly()
    {
        var serializer = new XSerializerXmlSerializer();

        using var stream = new MemoryStream();
        serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForXmlSerializer));

        using var streamReader = new StreamReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        var xml = streamReader.ReadToEnd();
        xml.Should().Be(string.Format(CultureInfo.CurrentCulture, EXPECTED_XML_FORMAT, STREAM_HEADER));
    }

    [Fact]
    public static void DeserializeFromStreamDeserializesCorrectly()
    {
        var serializer = new XSerializerXmlSerializer();

        TypeForXmlSerializer? item;
        using (var stream = new MemoryStream())
        {
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
            {
                writer.Write(EXPECTED_XML_FORMAT, STREAM_HEADER);
            }
            stream.Seek(0, SeekOrigin.Begin);

            item = serializer.DeserializeFromStream(stream, typeof(TypeForXmlSerializer)) as TypeForXmlSerializer;
        }

        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(_expectedItem);
    }

    [Fact]
    public static void SerializeToStringSerializesCorrectly()
    {
        var serializer = new XSerializerXmlSerializer();

        var xml = serializer.SerializeToString(_expectedItem, typeof(TypeForXmlSerializer));

        xml.Should().Be(string.Format(CultureInfo.CurrentCulture, EXPECTED_XML_FORMAT, STREAM_HEADER));
    }

    [Fact]
    public static void DeserializeFromStringDeserializesCorrectly()
    {
        var serializer = new XSerializerXmlSerializer();

        var item = serializer.DeserializeFromString(string.Format(CultureInfo.CurrentCulture, EXPECTED_XML_FORMAT, STRING_HEADER), typeof(TypeForXmlSerializer)) as TypeForXmlSerializer;

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
