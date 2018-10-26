using System;
using System.IO;
using System.Text;
using System.Xml;
using FluentAssertions;
using Xunit;

namespace RockLib.Serialization.Tests
{
    public class DefaultXmlSerializerTests
    {
        private readonly TypeForXmlSerializer _expectedItem = new TypeForXmlSerializer { PropA = 5, PropB = true, PropC = "PropC" };
        private readonly string _streamHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";
        private readonly string _stringHeader = @"<?xml version=""1.0"" encoding=""utf-16""?>";
        private readonly string _expectedXmlFormat = @"{0}<TypeForXmlSerializer xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><PropA>5</PropA><PropB>true</PropB><PropC>PropC</PropC></TypeForXmlSerializer>";

        [Fact]
        public void EmptyConstructorCreatesDefaultValues()
        {
            var serializer = new DefaultXmlSerializer();

            serializer.Name.Should().Be("default");
            serializer.WriterSettings.Should().BeNull();
            serializer.ReaderSettings.Should().BeNull();
            serializer.Namespaces.Should().BeNull();
        }

        [Fact]
        public void ConstructorWithNullNameCreatesDefaultName()
        {
            var serializer = new DefaultXmlSerializer(null);

            serializer.Name.Should().Be("default");
        }

        [Fact]
        public void ConstructorPassesValuesCorrectly()
        {
            var name = "notdefault";
            var xmlWriterSettings = new XmlWriterSettings();
            var xmlReaderSettings = new XmlReaderSettings();
            var nameSpaces = new[] { new XmlQualifiedName("Name1", "Namespace1"), new XmlQualifiedName("Name2", "Namespace2") };

            var serializer = new DefaultXmlSerializer(name, nameSpaces, xmlWriterSettings, xmlReaderSettings);

            serializer.Name.Should().Be(name);
            serializer.ReaderSettings.Should().BeSameAs(xmlReaderSettings);
            serializer.WriterSettings.Should().BeSameAs(xmlWriterSettings);
            serializer.Namespaces.ToArray().Should().BeEquivalentTo(nameSpaces);

        }

        [Fact]
        public void SerializeToStreamThrowWhenStreamIsNull()
        {
            Action action = () => new DefaultXmlSerializer().SerializeToStream(null, new object(), typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: stream");
        }

        [Fact]
        public void SerializeToStreamThrowWhenItemIsNull()
        {
            Action action = () => new DefaultXmlSerializer().SerializeToStream(new MemoryStream(), null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: item");
        }

        [Fact]
        public void SerializeToStreamThrowWhenTypeIsNull()
        {
            Action action = () => new DefaultXmlSerializer().SerializeToStream(new MemoryStream(), new object(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenStreamIsNull()
        {
            Action action = () => new DefaultXmlSerializer().DeserializeFromStream(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: stream");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenTypeIsNull()
        {
            Action action = () => new DefaultXmlSerializer().DeserializeFromStream(new MemoryStream(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void SerializeToStringThrowWhenItemIsNull()
        {
            Action action = () => new DefaultXmlSerializer().SerializeToString(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: item");
        }

        [Fact]
        public void SerializeToStringThrowWhenTypeIsNull()
        {
            Action action = () => new DefaultXmlSerializer().SerializeToString(new object(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenItemIsNull()
        {
            Action action = () => new DefaultXmlSerializer().DeserializeFromString(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: data");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenTypeIsNull()
        {
            Action action = () => new DefaultXmlSerializer().DeserializeFromString("", null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void SerializeToStreamSerializesCorrectly()
        {
            var serializer = new DefaultXmlSerializer(writerSettings: new XmlWriterSettings { Encoding = Encoding.Default});

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
            var serializer = new DefaultXmlSerializer();

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
            var serializer = new DefaultXmlSerializer(writerSettings: new XmlWriterSettings());

            var xml = serializer.SerializeToString(_expectedItem, typeof(TypeForXmlSerializer));

            xml.Should().Be(string.Format(_expectedXmlFormat, _stringHeader));
        }

        [Fact]
        public void DeserializeFromStringDeserializesCorrectly()
        {
            var serializer = new DefaultXmlSerializer();

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
