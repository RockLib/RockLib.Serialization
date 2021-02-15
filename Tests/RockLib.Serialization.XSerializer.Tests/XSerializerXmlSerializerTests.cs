using System;
using System.IO;
using System.Text;
using System.Xml;
using FluentAssertions;
using XSerializer;
using Xunit;

namespace RockLib.Serialization.XSerializer.Tests
{
    public class XSerializerXmlSerializerTests
    {
        private readonly TypeForXmlSerializer _expectedItem = new TypeForXmlSerializer { PropA = 5, PropB = true, PropC = "PropC" };
        private readonly string _streamHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";
        private readonly string _stringHeader = @"<?xml version=""1.0"" encoding=""utf-16""?>";
        private readonly string _expectedXmlFormat = @"{0}<TypeForXmlSerializer xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><PropA>5</PropA><PropB>true</PropB><PropC>PropC</PropC></TypeForXmlSerializer>";

        [Fact]
        public void EmptyConstructorCreatesDefaultValues()
        {
            var serializer = new XSerializerXmlSerializer();

            serializer.Name.Should().Be("default");
            serializer.Options.Should().BeNull();
        }

        [Fact]
        public void ConstructorWithNullNameCreatesDefaultName()
        {
            var serializer = new XSerializerXmlSerializer(null);

            serializer.Name.Should().Be("default");
        }

        [Fact]
        public void ConstructorPassesValuesCorrectly()
        {
            var name = "notdefault";
            var options = new XmlSerializationOptions(encoding: Encoding.UTF32);
            var serializer = new XSerializerXmlSerializer(name, options);

            serializer.Name.Should().Be(name);
            serializer.Options.Should().NotBeNull();
            serializer.Options.Should().BeSameAs(options);
        }

        [Fact]
        public void SerializeToStreamThrowWhenStreamIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().SerializeToStream(null, new object(), typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
        }

        [Fact]
        public void SerializeToStreamThrowWhenItemIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().SerializeToStream(new MemoryStream(), null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
        }

        [Fact]
        public void SerializeToStreamThrowWhenTypeIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().SerializeToStream(new MemoryStream(), new object(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenStreamIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().DeserializeFromStream(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*stream*");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenTypeIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().DeserializeFromStream(new MemoryStream(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
        }

        [Fact]
        public void SerializeToStringThrowWhenItemIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().SerializeToString(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*item*");
        }

        [Fact]
        public void SerializeToStringThrowWhenTypeIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().SerializeToString(new object(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenItemIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().DeserializeFromString(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*data*");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenTypeIsNull()
        {
            Action action = () => new XSerializerXmlSerializer().DeserializeFromString("", null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter*type*");
        }

        [Fact]
        public void SerializeToStreamSerializesCorrectly()
        {
            var serializer = new XSerializerXmlSerializer();

            using (var stream = new MemoryStream())
            {
                serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForXmlSerializer));

                using (var streamReader = new StreamReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var xml = streamReader.ReadToEnd();
                    xml.Should().Be(string.Format(_expectedXmlFormat, _streamHeader));
                }
            }
        }

        [Fact]
        public void DeserializeFromStreamDeserializesCorrectly()
        {
            var serializer = new XSerializerXmlSerializer();

            TypeForXmlSerializer item;
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
                {
                    writer.Write(_expectedXmlFormat, _streamHeader);
                }
                stream.Seek(0, SeekOrigin.Begin);

                item = serializer.DeserializeFromStream(stream, typeof(TypeForXmlSerializer)) as TypeForXmlSerializer;
            }

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(_expectedItem);
        }

        [Fact]
        public void SerializeToStringSerializesCorrectly()
        {
            var serializer = new XSerializerXmlSerializer();

            var xml = serializer.SerializeToString(_expectedItem, typeof(TypeForXmlSerializer));

            xml.Should().Be(string.Format(_expectedXmlFormat, _streamHeader));
        }

        [Fact]
        public void DeserializeFromStringDeserializesCorrectly()
        {
            var serializer = new XSerializerXmlSerializer();

            var item = serializer.DeserializeFromString(string.Format(_expectedXmlFormat, _stringHeader), typeof(TypeForXmlSerializer)) as TypeForXmlSerializer;

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(_expectedItem);
        }

        public class TypeForXmlSerializer
        {
            public int PropA { get; set; }
            public bool PropB { get; set; }
            public string PropC { get; set; }
        }
    }
}
