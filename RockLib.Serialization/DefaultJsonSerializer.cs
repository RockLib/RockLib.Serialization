﻿using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace RockLib.Serialization
{
    /// <summary>
    /// A JSON implementation of the <see cref="ISerializer"/> interface using <see cref="Newtonsoft.Json.JsonSerializer"/>.
    /// </summary>
    public class DefaultJsonSerializer : ISerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializer"/> class.
        /// </summary>
        /// <param name="name">The name of the serializer, used to when selecting which serializer to use.</param>
        /// <param name="settings">Newtonsoft settings for the serializer.</param>
        public DefaultJsonSerializer(string name = "default", JsonSerializerSettings settings = null)
        {
            Name = name ?? "default";

            JsonSerializer =
                settings == null
                    ? JsonSerializer.CreateDefault()
                    : JsonSerializer.Create(settings);
        }

        /// <inheritdoc />
        public string Name { get; }
        
        /// <summary>
        /// Gets the <see cref="JsonSerializer"/> used when serializing.
        /// </summary>
        public JsonSerializer JsonSerializer { get; }

        /// <inheritdoc />
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
                JsonSerializer.Serialize(jsonWriter, item);
        }

        /// <inheritdoc />
        public object DeserializeFromStream(Stream stream, Type type)
        {
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
                return JsonSerializer.Deserialize(jsonReader, type);
        }

        /// <inheritdoc />
        public string SerializeToString(object item, Type type)
        {
            var builder = new StringBuilder();

            using (var stringWriter = new StringWriter(builder))
            using (var jsonWriter = new JsonTextWriter(stringWriter))
                JsonSerializer.Serialize(jsonWriter, item, type);

            return builder.ToString();
        }

        /// <inheritdoc />
        public object DeserializeFromString(string data, Type type)
        {
            using (var stringReader = new StringReader(data))
            using (var jsonReader = new JsonTextReader(stringReader))
                return JsonSerializer.Deserialize(jsonReader, type);
        }
    }
}
