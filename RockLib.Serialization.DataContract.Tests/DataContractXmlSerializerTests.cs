using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using Xunit;

namespace RockLib.Serialization.DataContract.Tests
{
    public class DataContractXmlSerializerTests
    {
        private readonly TypeForXmlSerializer _expectedItem = new TypeForXmlSerializer { PropA = 5, PropB = true, PropC = "PropC" };
        private readonly string _streamHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";
        private readonly string _stringHeader = @"<?xml version=""1.0"" encoding=""utf-16""?>";
        private readonly string _expectedXmlFormat = @"{0}<DataContractXmlSerializerTests.TypeForXmlSerializer xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/RockLib.Serialization.DataContract.Tests""><PropA>5</PropA><PropB>true</PropB><PropC>PropC</PropC></DataContractXmlSerializerTests.TypeForXmlSerializer>";
        private readonly string _expectedStreamXml = @"<DataContractXmlSerializerTests.TypeForXmlSerializer xmlns=""http://schemas.datacontract.org/2004/07/RockLib.Serialization.DataContract.Tests"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><PropA>5</PropA><PropB>true</PropB><PropC>PropC</PropC></DataContractXmlSerializerTests.TypeForXmlSerializer>";

        [Fact]
        public void EmptyConstructorCreatesDefaultValues()
        {
            var serializer = new DataContractXmlSerializer();

            serializer.Name.Should().Be("default");
            serializer.Settings.Should().BeNull();
        }

        [Fact]
        public void ConstructorWithNullNameCreatesDefaultName()
        {
            var serializer = new DataContractXmlSerializer(null);

            serializer.Name.Should().Be("default");
        }

        [Fact]
        public void ConstructorPassesValuesCorrectly()
        {
            var name = "notdefault";
            var options = new DataContractSerializerSettings();

            var serializer = new DataContractXmlSerializer(name, options);

            serializer.Name.Should().Be(name);
            serializer.Settings.Should().NotBeNull();
            serializer.Settings.Should().BeSameAs(options);
        }

        [Fact]
        public void SerializeToStreamThrowWhenStreamIsNull()
        {
            Action action = () => new DataContractXmlSerializer().SerializeToStream(null, new object(), typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: stream");
        }

        [Fact]
        public void SerializeToStreamThrowWhenItemIsNull()
        {
            Action action = () => new DataContractXmlSerializer().SerializeToStream(new MemoryStream(), null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: item");
        }

        [Fact]
        public void SerializeToStreamThrowWhenTypeIsNull()
        {
            Action action = () => new DataContractXmlSerializer().SerializeToStream(new MemoryStream(), new object(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenStreamIsNull()
        {
            Action action = () => new DataContractXmlSerializer().DeserializeFromStream(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: stream");
        }

        [Fact]
        public void DeserializeFromStreamThrowWhenTypeIsNull()
        {
            Action action = () => new DataContractXmlSerializer().DeserializeFromStream(new MemoryStream(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void SerializeToStringThrowWhenItemIsNull()
        {
            Action action = () => new DataContractXmlSerializer().SerializeToString(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: item");
        }

        [Fact]
        public void SerializeToStringThrowWhenTypeIsNull()
        {
            Action action = () => new DataContractXmlSerializer().SerializeToString(new object(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenItemIsNull()
        {
            Action action = () => new DataContractXmlSerializer().DeserializeFromString(null, typeof(object));

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: data");
        }

        [Fact]
        public void DeserializeFromStringThrowWhenTypeIsNull()
        {
            Action action = () => new DataContractXmlSerializer().DeserializeFromString("", null);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: type");
        }

        [Fact]
        public void SerializeToStreamSerializesCorrectly()
        {
            var serializer = new DataContractXmlSerializer();

            using (var stream = new MemoryStream())
            {
                serializer.SerializeToStream(stream, _expectedItem, typeof(TypeForXmlSerializer));

                using (var streamReader = new StreamReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var xml = streamReader.ReadToEnd();
                    xml.Should().Be(_expectedStreamXml);
                }
            }
        }

        [Fact]
        public void DeserializeFromStreamDeserializesCorrectly()
        {
            var serializer = new DataContractXmlSerializer();

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
            var serializer = new DataContractXmlSerializer();

            var xml = serializer.SerializeToString(_expectedItem, typeof(TypeForXmlSerializer));

            xml.Should().Be(string.Format(_expectedXmlFormat, _stringHeader));
        }

        [Fact]
        public void DeserializeFromStringDeserializesCorrectly()
        {
            var serializer = new DataContractXmlSerializer();

            var item = serializer.DeserializeFromString(string.Format(_expectedXmlFormat, _stringHeader), typeof(TypeForXmlSerializer)) as TypeForXmlSerializer;

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(_expectedItem);
        }

        [DataContract]
        public class TypeForXmlSerializer
        {
            [DataMember]
            public int PropA { get; set; }
            [DataMember]
            public bool PropB { get; set; }
            [DataMember]
            public string PropC { get; set; }
        }
    }
}
