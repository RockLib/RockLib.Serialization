using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RockLib.Serialization
{
    public class DefaultXmlSerializer : ISerializer
    {
        private readonly XmlWriterSettings _writerSettings;
        private readonly XmlReaderSettings _readerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializer"/> class.
        /// </summary>
        /// <param name="name">The name of the serializer, used to when selecting which serializer to use.</param>
        /// <param name="writerSettings">XmlWriterSettings settings for the serializer.</param>
        /// <param name="readerSettings">XmlReaderSettings settings for the serializer.</param>
        public DefaultXmlSerializer(string name = "default", XmlWriterSettings writerSettings = null, XmlReaderSettings readerSettings = null)
        {
            Name = name;
            _writerSettings = writerSettings;
            _readerSettings = readerSettings;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            type = CheckType(type, item);

            if (_writerSettings == null)
                new XmlSerializer(type).Serialize(stream, item);
            else
                using (var xmlWriter = XmlWriter.Create(stream, _writerSettings))
                    new XmlSerializer(type).Serialize(xmlWriter, item);
        }

        /// <inheritdoc />
        public object DeserializeFromStream(Stream stream, Type type)
        {
            if (_readerSettings == null)
                return new XmlSerializer(type).Deserialize(stream);

            using (var xmlReader = XmlReader.Create(stream, _readerSettings))
                return new XmlSerializer(type).Deserialize(xmlReader);
        }

        /// <inheritdoc />
        public string SerializeToString(object item, Type type)
        {
            type = CheckType(type, item);

            var builder = new StringBuilder();
            if (_writerSettings == null)
                using (var writer = new StringWriter(builder))
                    new XmlSerializer(type).Serialize(writer, item);
            else
                using (var writer = XmlWriter.Create(builder, _writerSettings))
                    new XmlSerializer(type).Serialize(writer, item);

            return builder.ToString();
        }

        /// <inheritdoc />
        public object DeserializeFromString(string data, Type type)
        {
            if (_readerSettings == null)
                using (var reader = new StringReader(data))
                    return new XmlSerializer(type).Deserialize(reader);

            using (var reader = new StringReader(data))
            using (var xmlReader = XmlReader.Create(reader, _readerSettings))
                return new XmlSerializer(type).Deserialize(xmlReader);
        }

        /// <summary>
        /// If <paramref name="type"/> is abstract, return item.GetType().
        /// If <paramref name="type"/> is not abstract, return it.
        /// </summary>
        /// <remarks>
        /// This check allows us to handle an abstract type during
        /// serialization. There's still nothing that can be done when
        /// deserializing.
        /// </remarks>
        private static Type CheckType(Type type, object item)
        {
            return !type.IsAbstract ? type : item.GetType();
        }
    }
}
