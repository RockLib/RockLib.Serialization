using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using Xunit;

namespace RockLib.Serialization.DataContract.Tests;

public static class DataContractXmlSerializerTests
{
    private static readonly TypeForXmlSerializer _expectedItem = new() { PropA = 5, PropB = true, PropC = "PropC" };
    private const string STREAM_HEADER = @"<?xml version=""1.0"" encoding=""utf-8""?>";
    private const string STRING_HEADER = @"<?xml version=""1.0"" encoding=""utf-16""?>";
    private const string EXPECTED_XML_FORMAT = @"{0}<DataContractXmlSerializerTests.TypeForXmlSerializer xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/RockLib.Serialization.DataContract.Tests""><PropA>5</PropA><PropB>true</PropB><PropC>PropC</PropC></DataContractXmlSerializerTests.TypeForXmlSerializer>";
    private const string EXPECTED_STREAM_XML = @"<DataContractXmlSerializerTests.TypeForXmlSerializer xmlns=""http://schemas.datacontract.org/2004/07/RockLib.Serialization.DataContract.Tests"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><PropA>5</PropA><PropB>true</PropB><PropC>PropC</PropC></DataContractXmlSerializerTests.TypeForXmlSerializer>";

    [Fact]
    public static void EmptyConstructorCreatesDefaultValues()
    {
        var serializer = new DataContractXmlSerializer();

        serializer.Name.Should().Be("default");
        serializer.Settings.Should().BeNull();
    }

    [Fact]
    public static void ConstructorWithNullNameCreatesDefaultName()
    {
        var serializer = new DataContractXmlSerializer(null!);

        serializer.Name.Should().Be("default");
    }

    [Fact]
    public static void ConstructorPassesValuesCorrectly()
    {
        var name = "notdefault";
        var options = new DataContractSerializerSettings();

        var serializer = new DataContractXmlSerializer(name, options);

        serializer.Name.Should().Be(name);
        serializer.Settings.Should().NotBeNull();
        serializer.Settings.Should().BeSameAs(options);
    }

    [Fact]
    public static void SerializeToStreamThrowWhenStreamIsNull()
    {
        Action action = () => new DataContractXmlSerializer().SerializeToStream(null!, new object(), typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenItemIsNull()
    {
        Action action = () => new DataContractXmlSerializer().SerializeToStream(new MemoryStream(), null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenTypeIsNull()
    {
        Action action = () => new DataContractXmlSerializer().SerializeToStream(new MemoryStream(), new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenStreamIsNull()
    {
        Action action = () => new DataContractXmlSerializer().DeserializeFromStream(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenTypeIsNull()
    {
        Action action = () => new DataContractXmlSerializer().DeserializeFromStream(new MemoryStream(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenItemIsNull()
    {
        Action action = () => new DataContractXmlSerializer().SerializeToString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenTypeIsNull()
    {
        Action action = () => new DataContractXmlSerializer().SerializeToString(new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenItemIsNull()
    {
        Action action = () => new DataContractXmlSerializer().DeserializeFromString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*data*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenTypeIsNull()
    {
        Action action = () => new DataContractXmlSerializer().DeserializeFromString("", null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStreamSerializesCorrectly()
    {
        var serializer = new DataContractXmlSerializer();

        using var stream = new MemoryStream();
        serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForXmlSerializer));

        using var streamReader = new StreamReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        var xml = streamReader.ReadToEnd();
        xml.Should().Be(EXPECTED_STREAM_XML);
    }

    [Fact]
    public static void DeserializeFromStreamDeserializesCorrectly()
    {
        var serializer = new DataContractXmlSerializer();

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
        var serializer = new DataContractXmlSerializer();

        var xml = serializer.SerializeToString(_expectedItem, typeof(TypeForXmlSerializer));

        xml.Should().Be(string.Format(CultureInfo.CurrentCulture, EXPECTED_XML_FORMAT, STRING_HEADER));
    }

    [Fact]
    public static void DeserializeFromStringDeserializesCorrectly()
    {
        var serializer = new DataContractXmlSerializer();

        var item = serializer.DeserializeFromString(string.Format(CultureInfo.CurrentCulture, EXPECTED_XML_FORMAT, STRING_HEADER), typeof(TypeForXmlSerializer)) as TypeForXmlSerializer;

        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(_expectedItem);
    }

    [DataContract]
#pragma warning disable CA1034 // Nested types should not be visible
    public class TypeForXmlSerializer
#pragma warning restore CA1034 // Nested types should not be visible
    {
        [DataMember]
        public int PropA { get; set; }
        [DataMember]
        public bool PropB { get; set; }
        [DataMember]
        public string? PropC { get; set; }
    }
}
