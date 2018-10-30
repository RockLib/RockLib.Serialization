using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Newtonsoft.Json;
using XSerializer;
using Xunit;

namespace RockLib.Serialization.XSerializer.Tests
{
    public class XSerializerJsonSerializerTests
    {
        private readonly string _expectedJson;
        private readonly TypeForJsonSerializer _expectedItem;

        public XSerializerJsonSerializerTests()
        {
            _expectedItem = new TypeForJsonSerializer { PropA = 5, PropB = true, PropC = "PropC" };
            _expectedJson = @"{""PropA"":5,""PropB"":true,""PropC"":""PropC""}";
        }

        [Fact]
        public void EmptyConstructorCreatesDefaultValues()
        {
            var serializer = new XSerializerJsonSerializer();

            serializer.Name.Should().Be("default");
            serializer.Options.Should().BeNull();
        }

        [Fact]
        public void ConstructorWithNullNameCreatesDefaultName()
        {
            var serializer = new XSerializerJsonSerializer(null);

            serializer.Name.Should().Be("default");
        }

        [Fact]
        public void ConstructorPassesValuesCorrectly()
        {
            var name = "notdefault";
            var options = new JsonSerializerConfiguration { Encoding = Encoding.UTF32 };

            var serializer = new XSerializerJsonSerializer(name, options);

            serializer.Name.Should().Be(name);
            serializer.Options.Should().BeSameAs(options);
        }

        [Fact]
        public void SerializeToStreamThrowWhenStreamIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().SerializeToStream(null, new object(), typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: stream");
        }

        [Fact]
        public void SerializeToStreamThrowWhenItemIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().SerializeToStream(new MemoryStream(), null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: item");
        }

        [Fact]
        public void SerializeToStreamThrowWhenTypeIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().SerializeToStream(new MemoryStream(), new object(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenStreamIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().DeserializeFromStream(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: stream");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenTypeIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().DeserializeFromStream(new MemoryStream(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void SerializeToStringThrowWhenItemIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().SerializeToString(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: item");
        }

        [Fact]
        public void SerializeToStringThrowWhenTypeIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().SerializeToString(new object(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenItemIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().DeserializeFromString(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: data");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenTypeIsNull()
        {
            Action action = () => new XSerializerJsonSerializer().DeserializeFromString("", null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void SerializeToStreamSerializesCorrectly()
        {
            var serializer = new XSerializerJsonSerializer();

            using (var stream = new MemoryStream())
            {
                serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForJsonSerializer));

                stream.Flush();
                var data = stream.ToArray();

                using (var readStream = new MemoryStream(data))
                {
                    using (var reader = new StreamReader(readStream))
                    {
                        var json = reader.ReadToEnd();
                        json.Should().Be(_expectedJson);
                    }
                }
            }
        }

        [Fact]
        public void DeserializeFromStreamDeserializesCorrectly()
        {
            var serializer = new XSerializerJsonSerializer();

            TypeForJsonSerializer item;
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
                {
                    writer.Write(_expectedJson);
                }
                stream.Seek(0, SeekOrigin.Begin);

                item = serializer.DeserializeFromStream(stream, typeof(TypeForJsonSerializer)) as TypeForJsonSerializer;
            }

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(_expectedItem);
        }

        [Fact]
        public void SerializeToStringSerializesCorrectly()
        {
            var serializer = new XSerializerJsonSerializer();

            var json = serializer.SerializeToString(_expectedItem, typeof(TypeForJsonSerializer));

            json.Should().Be(_expectedJson);
        }

        [Fact]
        public void DeserializeFromStringDeserializesCorrectly()
        {
            var serializer = new XSerializerJsonSerializer();

            var item = serializer.DeserializeFromString(_expectedJson, typeof(TypeForJsonSerializer)) as TypeForJsonSerializer;

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(_expectedItem);
        }

        public class TypeForJsonSerializer
        {
            public int PropA { get; set; }
            public bool PropB { get; set; }
            public string PropC { get; set; }
        }
    }
}
