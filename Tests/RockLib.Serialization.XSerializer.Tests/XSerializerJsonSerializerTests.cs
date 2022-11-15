using System;
using System.IO;
using System.Text;
using FluentAssertions;
using XSerializer;
using Xunit;

namespace RockLib.Serialization.XSerializer.Tests;

public static class XSerializerJsonSerializerTests
{
    private const string EXPECTED_JSON = @"{""PropA"":5,""PropB"":true,""PropC"":""PropC""}";
    private static readonly TypeForJsonSerializer _expectedItem = new() { PropA = 5, PropB = true, PropC = "PropC" };

    [Fact]
    public static void EmptyConstructorCreatesDefaultValues()
    {
        var serializer = new XSerializerJsonSerializer();

        serializer.Name.Should().Be("default");
        serializer.Options.Should().BeNull();
    }

    [Fact]
    public static void ConstructorWithNullNameCreatesDefaultName()
    {
        var serializer = new XSerializerJsonSerializer(null!);

        serializer.Name.Should().Be("default");
    }

    [Fact]
    public static void ConstructorPassesValuesCorrectly()
    {
        var name = "notdefault";
        var options = new JsonSerializerConfiguration { Encoding = Encoding.UTF32 };

        var serializer = new XSerializerJsonSerializer(name, options);

        serializer.Name.Should().Be(name);
        serializer.Options.Should().BeSameAs(options);
    }

    [Fact]
    public static void SerializeToStreamThrowWhenStreamIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().SerializeToStream(null!, new object(), typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenItemIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().SerializeToStream(new MemoryStream(), null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenTypeIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().SerializeToStream(new MemoryStream(), new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenStreamIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().DeserializeFromStream(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenTypeIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().DeserializeFromStream(new MemoryStream(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenItemIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().SerializeToString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenTypeIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().SerializeToString(new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenItemIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().DeserializeFromString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*data*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenTypeIsNull()
    {
        Action action = () => new XSerializerJsonSerializer().DeserializeFromString("", null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStreamSerializesCorrectly()
    {
        var serializer = new XSerializerJsonSerializer();

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
        var serializer = new XSerializerJsonSerializer();

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
        var serializer = new XSerializerJsonSerializer();

        var json = serializer.SerializeToString(_expectedItem, typeof(TypeForJsonSerializer));

        json.Should().Be(EXPECTED_JSON);
    }

    [Fact]
    public static void DeserializeFromStringDeserializesCorrectly()
    {
        var serializer = new XSerializerJsonSerializer();

        var item = serializer.DeserializeFromString(EXPECTED_JSON, typeof(TypeForJsonSerializer)) as TypeForJsonSerializer;

        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(_expectedItem);
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public class TypeForJsonSerializer
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public int PropA { get; set; }
        public bool PropB { get; set; }
        public string? PropC { get; set; }
    }
}
