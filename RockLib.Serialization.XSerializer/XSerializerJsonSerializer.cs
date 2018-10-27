using System;
using System.IO;
using XSerializer;

namespace RockLib.Serialization.XSerializer
{
    /// <summary>
    /// A JSON implementation of the <see cref="ISerializer"/> interface using <see cref="JsonSerializer"/>/>.
    /// </summary>
    public class XSerializerJsonSerializer : ISerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XSerializerJsonSerializer"/> class.
        /// </summary>
        /// <param name="name">The name of the serializer, used to when selecting which serializer to use.</param>
        /// <param name="options">Options for customizing the JsonSerializer.</param>
        public XSerializerJsonSerializer(string name = "default", JsonSerializerConfiguration options = null)
        {
            Name = name ?? "default";
            Options = options;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="JsonSerializerConfiguration"/> options.
        /// </summary>
        public JsonSerializerConfiguration Options { get; }
        
        /// <inheritdoc />
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var serializer = JsonSerializer.Create(type, Options);
            serializer.Serialize(stream, item);
        }

        /// <inheritdoc />
        public object DeserializeFromStream(Stream stream, Type type)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var serializer = JsonSerializer.Create(type, Options);
            return serializer.Deserialize(stream);
        }

        /// <inheritdoc />
        public string SerializeToString(object item, Type type)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var serializer = JsonSerializer.Create(type, Options);
            return serializer.Serialize(item);
        }

        /// <inheritdoc />
        public object DeserializeFromString(string data, Type type)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var serializer = JsonSerializer.Create(type, Options);
            return serializer.Deserialize(data);
        }
    }
}
