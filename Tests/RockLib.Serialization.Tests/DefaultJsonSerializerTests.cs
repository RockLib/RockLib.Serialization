using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace RockLib.Serialization.Tests
{
    public class DefaultJsonSerializerTests
    {
        private readonly string _expectedJson;
        private readonly TypeForJsonSerializer _expectedItem;

        public DefaultJsonSerializerTests()
        {
            _expectedItem = new TypeForJsonSerializer { PropA = 5, PropB = true, PropC = "PropC" };
            _expectedJson = @"{""PropA"":5,""PropB"":true,""PropC"":""PropC""}";
        }

        [Fact]
        public void EmptyConstructorCreatesDefaultValues()
        {
            var serializer = new DefaultJsonSerializer();

            serializer.Name.Should().Be("default");
            serializer.JsonSerializer.Should().NotBeNull();
            serializer.JsonSerializer.Formatting.Should().Be(Formatting.None);
        }

        [Fact]
        public void ConstructorWithNullNameCreatesDefaultName()
        {
            var serializer = new DefaultJsonSerializer(null!);

            serializer.Name.Should().Be("default");
        }

        [Fact]
        public void ConstructorPassesValuesCorrectly()
        {
            var serializer = new DefaultJsonSerializer("notdefault", new JsonSerializerSettings() { Formatting = Formatting.Indented });

            serializer.Name.Should().Be("notdefault");
            serializer.JsonSerializer.Should().NotBeNull();
            serializer.JsonSerializer.Formatting.Should().Be(Formatting.Indented);
        }

        [Fact]
        public void SerializeToStreamThrowWhenStreamIsNull()
        {
            Action action = () => new DefaultJsonSerializer().SerializeToStream(null!, new object(), typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
        }

        [Fact]
        public void SerializeToStreamThrowWhenItemIsNull()
        {
            Action action = () => new DefaultJsonSerializer().SerializeToStream(new MemoryStream(), null!, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
        }

        [Fact]
        public void SerializeToStreamThrowWhenTypeIsNull()
        {
            Action action = () => new DefaultJsonSerializer().SerializeToStream(new MemoryStream(), new object(), null!);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenStreamIsNull()
        {
            Action action = () => new DefaultJsonSerializer().DeserializeFromStream(null!, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenTypeIsNull()
        {
            Action action = () => new DefaultJsonSerializer().DeserializeFromStream(new MemoryStream(), null!);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
        }

        [Fact]
        public void SerializeToStringThrowWhenItemIsNull()
        {
            Action action = () => new DefaultJsonSerializer().SerializeToString(null!, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
        }

        [Fact]
        public void SerializeToStringThrowWhenTypeIsNull()
        {
            Action action = () => new DefaultJsonSerializer().SerializeToString(new object(), null!);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenItemIsNull()
        {
            Action action = () => new DefaultJsonSerializer().DeserializeFromString(null!, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*data*");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenTypeIsNull()
        {
            Action action = () => new DefaultJsonSerializer().DeserializeFromString("", null!);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
        }

        [Fact]
        public void SerializeToStreamSerializesCorrectly()
        {
            var serializer = new DefaultJsonSerializer();

            using var stream = new MemoryStream();
            serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForJsonSerializer));

            using var streamReader = new StreamReader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var json = streamReader.ReadToEnd();
            json.Should().Be(_expectedJson);
        }

        [Fact]
        public void DeserializeFromStreamDeserializesCorrectly()
        {
            var serializer = new DefaultJsonSerializer();

            TypeForJsonSerializer item;
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
                {
                    writer.Write(_expectedJson);
                }
                stream.Seek(0, SeekOrigin.Begin);

                item = (TypeForJsonSerializer)serializer.DeserializeFromStream(stream, typeof(TypeForJsonSerializer))!;
            }

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(_expectedItem);
        }

        [Fact]
        public void SerializeToStringSerializesCorrectly()
        {
            var serializer = new DefaultJsonSerializer();

            var json = serializer.SerializeToString(_expectedItem, typeof(TypeForJsonSerializer));

            json.Should().Be(_expectedJson);
        }

        [Fact]
        public void DeserializeFromStringDeserializesCorrectly()
        {
            var serializer = new DefaultJsonSerializer();

            var item = serializer.DeserializeFromString(_expectedJson, typeof(TypeForJsonSerializer)) as TypeForJsonSerializer;

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
}
