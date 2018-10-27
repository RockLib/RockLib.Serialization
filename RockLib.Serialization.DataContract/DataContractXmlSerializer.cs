using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace RockLib.Serialization.DataContract
{
    /// <summary>
    /// An XML implementation of the <see cref="ISerializer"/> interface using <see cref="DataContractSerializer"/>/>.
    /// </summary>
    public class DataContractXmlSerializer : ISerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractXmlSerializer"/> class.
        /// </summary>
        /// <param name="name">The name of the serializer, used to when selecting which serializer to use.</param>
        /// <param name="settings">Settings for customizing the XmlSerializer.</param>
        public DataContractXmlSerializer(string name = "default", DataContractSerializerSettings settings = null)
        {
            Name = name ?? "default";
            Settings = settings;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="DataContractSerializerSettings"/> settings.
        /// </summary>
        public DataContractSerializerSettings Settings { get; }

        /// <inheritdoc />
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var serializer = Settings == null
                ? new DataContractSerializer(type)
                : new DataContractSerializer(type, Settings);

            serializer.WriteObject(stream, item);
        }

        /// <inheritdoc />
        public object DeserializeFromStream(Stream stream, Type type)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var serializer = Settings == null
                ? new DataContractSerializer(type)
                : new DataContractSerializer(type, Settings);

            return serializer.ReadObject(stream);
        }

        /// <inheritdoc />
        public string SerializeToString(object item, Type type)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var serializer = Settings == null
                ? new DataContractSerializer(type)
                : new DataContractSerializer(type, Settings);

            var sb = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(sb))
                serializer.WriteObject(xmlWriter, item);

            return sb.ToString();
        }

        /// <inheritdoc />
        public object DeserializeFromString(string data, Type type)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var serializer = Settings == null
                ? new DataContractSerializer(type)
                : new DataContractSerializer(type, Settings);

            using (var stringReader = new StringReader(data))
                using (var xmlReader = XmlReader.Create(stringReader))
                    return serializer.ReadObject(xmlReader);
        }
    }
}
