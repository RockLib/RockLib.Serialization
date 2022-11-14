using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using FluentAssertions;
using Xunit;

namespace RockLib.Serialization.DataContract.Tests;

public static class DataContractJsonSerializerTests
{
    private const string EXPECTED_JSON = @"{""PropA"":5,""PropB"":true,""PropC"":""PropC""}";
    private static readonly TypeForJsonSerializer _expectedItem = new() { PropA = 5, PropB = true, PropC = "PropC" };

    [Fact]
    public static void EmptyConstructorCreatesDefaultValues()
    {
        var serializer = new DataContractJsonSerializer();

        serializer.Name.Should().Be("default");
        serializer.Settings.Should().BeNull();
    }

    [Fact]
    public static void ConstructorWithNullNameCreatesDefaultName()
    {
        var serializer = new DataContractJsonSerializer(null!);

        serializer.Name.Should().Be("default");
    }

    [Fact]
    public static void ConstructorPassesValuesCorrectly()
    {
        var name = "notdefault";
        var options = new DataContractJsonSerializerSettings();

        var serializer = new DataContractJsonSerializer(name, options);

        serializer.Name.Should().Be(name);
        serializer.Settings.Should().NotBeNull();
        serializer.Settings.Should().BeSameAs(options);
    }

    [Fact]
    public static void SerializeToStreamThrowWhenStreamIsNull()
    {
        Action action = () => new DataContractJsonSerializer().SerializeToStream(null!, new object(), typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenItemIsNull()
    {
        Action action = () => new DataContractJsonSerializer().SerializeToStream(new MemoryStream(), null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenTypeIsNull()
    {
        Action action = () => new DataContractJsonSerializer().SerializeToStream(new MemoryStream(), new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenStreamIsNull()
    {
        Action action = () => new DataContractJsonSerializer().DeserializeFromStream(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenTypeIsNull()
    {
        Action action = () => new DataContractJsonSerializer().DeserializeFromStream(new MemoryStream(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenItemIsNull()
    {
        Action action = () => new DataContractJsonSerializer().SerializeToString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenTypeIsNull()
    {
        Action action = () => new DataContractJsonSerializer().SerializeToString(new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenItemIsNull()
    {
        Action action = () => new DataContractJsonSerializer().DeserializeFromString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*data*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenTypeIsNull()
    {
        Action action = () => new DataContractJsonSerializer().DeserializeFromString("", null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStreamSerializesCorrectly()
    {
        var serializer = new DataContractJsonSerializer();

        using var stream = new MemoryStream();
        serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForJsonSerializer));

        stream.Flush();
        var data = stream.ToArray();

        using var readStream = new MemoryStream(data);
        using var reader = new StreamReader(readStream);
        var json = reader.ReadToEnd();
        json.Should().Be(EXPECTED_JSON);
    }

    [Fact]
    public static void DeserializeFromStreamDeserializesCorrectly()
    {
        var serializer = new DataContractJsonSerializer();

        TypeForJsonSerializer? item;
        using (var stream = new MemoryStream())
        {
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
            {
                writer.Write(EXPECTED_JSON);
            }
            stream.Seek(0, SeekOrigin.Begin);

            item = serializer.DeserializeFromStream(stream, typeof(TypeForJsonSerializer)) as TypeForJsonSerializer;
        }

        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(_expectedItem);
    }

    [Fact]
    public static void SerializeToStringSerializesCorrectly()
    {
        var serializer = new DataContractJsonSerializer();

        var json = serializer.SerializeToString(_expectedItem, typeof(TypeForJsonSerializer));

        json.Should().Be(EXPECTED_JSON);
    }

    [Fact]
    public static void DeserializeFromStringDeserializesCorrectly()
    {
        var serializer = new DataContractJsonSerializer();

        var item = serializer.DeserializeFromString(EXPECTED_JSON, typeof(TypeForJsonSerializer)) as TypeForJsonSerializer;

        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(_expectedItem);
    }

    [DataContract]
#pragma warning disable CA1034 // Nested types should not be visible
    public class TypeForJsonSerializer
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
