using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace RockLib.Serialization.Tests;

public static class DefaultJsonSerializerTests
{
    private const string EXPECTED_JSON = @"{""PropA"":5,""PropB"":true,""PropC"":""PropC""}";
    private static TypeForJsonSerializer _expectedItem = new() { PropA = 5, PropB = true, PropC = "PropC" };

    [Fact]
    public static void EmptyConstructorCreatesDefaultValues()
    {
        var serializer = new DefaultJsonSerializer();

        serializer.Name.Should().Be("default");
        serializer.JsonSerializer.Should().NotBeNull();
        serializer.JsonSerializer.Formatting.Should().Be(Formatting.None);
    }

    [Fact]
    public static void ConstructorWithNullNameCreatesDefaultName()
    {
        var serializer = new DefaultJsonSerializer(null!);

        serializer.Name.Should().Be("default");
    }

    [Fact]
    public static void ConstructorPassesValuesCorrectly()
    {
        var serializer = new DefaultJsonSerializer("notdefault", new JsonSerializerSettings() { Formatting = Formatting.Indented });

        serializer.Name.Should().Be("notdefault");
        serializer.JsonSerializer.Should().NotBeNull();
        serializer.JsonSerializer.Formatting.Should().Be(Formatting.Indented);
    }

    [Fact]
    public static void SerializeToStreamThrowWhenStreamIsNull()
    {
        Action action = () => new DefaultJsonSerializer().SerializeToStream(null!, new object(), typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenItemIsNull()
    {
        Action action = () => new DefaultJsonSerializer().SerializeToStream(new MemoryStream(), null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStreamThrowWhenTypeIsNull()
    {
        Action action = () => new DefaultJsonSerializer().SerializeToStream(new MemoryStream(), new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenStreamIsNull()
    {
        Action action = () => new DefaultJsonSerializer().DeserializeFromStream(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
    }

    [Fact]
    public static void DeserializeFromStreamThrowWhenTypeIsNull()
    {
        Action action = () => new DefaultJsonSerializer().DeserializeFromStream(new MemoryStream(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenItemIsNull()
    {
        Action action = () => new DefaultJsonSerializer().SerializeToString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
    }

    [Fact]
    public static void SerializeToStringThrowWhenTypeIsNull()
    {
        Action action = () => new DefaultJsonSerializer().SerializeToString(new object(), null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenItemIsNull()
    {
        Action action = () => new DefaultJsonSerializer().DeserializeFromString(null!, typeof(object));

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*data*");
    }

    [Fact]
    public static void DeserializeFromStringThrowWhenTypeIsNull()
    {
        Action action = () => new DefaultJsonSerializer().DeserializeFromString("", null!);

        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
    }

    [Fact]
    public static void SerializeToStreamSerializesCorrectly()
    {
        var serializer = new DefaultJsonSerializer();

        using var stream = new MemoryStream();
        serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForJsonSerializer));

        using var streamReader = new StreamReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        var json = streamReader.ReadToEnd();
        json.Should().Be(EXPECTED_JSON);
    }

    [Fact]
    public static void DeserializeFromStreamDeserializesCorrectly()
    {
        var serializer = new DefaultJsonSerializer();

        TypeForJsonSerializer item;
        using (var stream = new MemoryStream())
        {
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
            {
                writer.Write(EXPECTED_JSON);
            }
            stream.Seek(0, SeekOrigin.Begin);

            item = (TypeForJsonSerializer)serializer.DeserializeFromStream(stream, typeof(TypeForJsonSerializer))!;
        }

        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(_expectedItem);
    }

    [Fact]
    public static void SerializeToStringSerializesCorrectly()
    {
        var serializer = new DefaultJsonSerializer();

        var json = serializer.SerializeToString(_expectedItem, typeof(TypeForJsonSerializer));

        json.Should().Be(EXPECTED_JSON);
    }

    [Fact]
    public static void DeserializeFromStringDeserializesCorrectly()
    {
        var serializer = new DefaultJsonSerializer();

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
