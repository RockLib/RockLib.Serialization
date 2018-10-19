using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Immutable;

namespace RockLib.Serialization
{
    /// <summary>
    /// Provides a set of static methods for serialzing and deserializing.
    /// </summary>
    public static class Serializer
    {
        private static readonly Semimutable<Dictionary<string, ISerializer>> _jsonSerializers = new Semimutable<Dictionary<string, ISerializer>>(LoadJsonSerializers);
        private static readonly Semimutable<Dictionary<string, ISerializer>> _xmlSerializers = new Semimutable<Dictionary<string, ISerializer>>(LoadXmlSerializers);

        /// <summary>
        /// Gets the JSON serializers.
        /// </summary>
        public static IReadOnlyList<ISerializer> JsonSerializers => _jsonSerializers.Value.Values.ToList();

        /// <summary>
        /// Gets the XML serializers.
        /// </summary>
        public static IReadOnlyList<ISerializer> XmlSerializers => _xmlSerializers.Value.Values.ToList();

        private static Dictionary<string, ISerializer> LoadJsonSerializers()
        {
            var serializers = Config.Root.GetSection("RockLib.Serialization:JsonSerializers")
                .Create<List<ISerializer>>(new DefaultTypes().Add(typeof(ISerializer), typeof(DefaultJsonSerializer)));

            return serializers == null || serializers.Count == 0
                ? new Dictionary<string, ISerializer> { { "default", new DefaultJsonSerializer() } }
                : serializers.ToDictionary(s => s.Name);
        }

        private static Dictionary<string, ISerializer> LoadXmlSerializers()
        {
            var serializers = Config.Root.GetSection("RockLib.Serialization:XmlSerializers")
                .Create<IReadOnlyList<ISerializer>>(new DefaultTypes().Add(typeof(ISerializer), typeof(DefaultXmlSerializer)));

            return serializers == null || serializers.Count == 0
                ? new Dictionary<string, ISerializer> { { "default", new DefaultXmlSerializer() } }
                : serializers.ToDictionary(s => s.Name);
        }

        /// <summary>
        /// Sets the JSON serializers.
        /// NOTE: This can only be used until the serializers have been used, then it will be locked and throw an exception.
        /// </summary>
        /// <param name="serializers">The JSON serializers to be used in serialization and deserialization.</param>
        public static void SetJsonSerializers(IEnumerable<ISerializer> serializers)
            => _jsonSerializers.Value = serializers?.ToDictionary(s => s.Name) ?? throw new ArgumentNullException(nameof(serializers));

        /// <summary>
        /// Sets the XML serializers.
        /// NOTE: This can only be used until the serializers have been used, then it will be locked and throw an exception.
        /// </summary>
        /// <param name="serializers">The XML serializers to be used in serialization and deserialization.</param>
        public static void SetXmlSerializers(IEnumerable<ISerializer> serializers)
            => _xmlSerializers.Value = serializers?.ToDictionary(s => s.Name) ?? throw new ArgumentNullException(nameof(serializers));

        /// <summary>
        /// Serializes an object of type T into a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of object being serialized.</typeparam>
        /// <param name="item">The object to serialize.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>A JSON string representing the serialized object.</returns>
        public static string ToJson<T>(this T item, string name = "default")
            => ToJson(item, typeof(T), name);

        /// <summary>
        /// Serializes an object of <paramref name="type"/> into a JSON string.
        /// </summary>
        /// <param name="item">The object to serialize.</param>
        /// <param name="type">The type of the object being serialized.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>A JSON string representing the serialized object.</returns>
        public static string ToJson(this object item, Type type, string name = "default")
            => _jsonSerializers.Value[name].SerializeToString(item, type);

        /// <summary>
        /// Serializes an object of type T into a JSON stream.
        /// </summary>
        /// <typeparam name="T">The type of object being serialized.</typeparam>
        /// <param name="item">The object to serialize.</param>
        /// <param name="stream">The stream to serialize into.</param>
        /// <param name="name">The name of the serializer.</param>
        public static void ToJson<T>(this T item, Stream stream, string name = "default")
            => ToJson(item, typeof(T), stream, name);

        /// <summary>
        /// Serializes an object of <paramref name="type"/> into a JSON stream.
        /// </summary>
        /// <param name="item">The object to serialize.</param>
        /// <param name="type">The type of the object being serialized.</param>
        /// <param name="stream">The stream to serialize into.</param>
        /// <param name="name">The name of the serializer.</param>
        public static void ToJson(this object item, Type type, Stream stream, string name = "default")
            => _jsonSerializers.Value[name].SerializeToStream(stream, item, type);

        /// <summary>
        /// Serializes an object of type T into a XML string.
        /// </summary>
        /// <typeparam name="T">The type of object being serialized.</typeparam>
        /// <param name="item">The object to serialize.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>An XML string representing the serialized object.</returns>
        public static string ToXml<T>(this T item, string name = "default")
            => ToXml(item, typeof(T), name);

        /// <summary>
        /// Serializes an object of <paramref name="type"/> into a XML string.
        /// </summary>
        /// <param name="item">The object to serialize.</param>
        /// <param name="type">The type of the object being serialized.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>An XML string representing the serialized object.</returns>
        public static string ToXml(this object item, Type type, string name = "default")
            => _xmlSerializers.Value[name].SerializeToString(item, type);

        /// <summary>
        /// Serializes an object of type T into a XML string.
        /// </summary>
        /// <typeparam name="T">The type of object being serialized.</typeparam>
        /// <param name="item">The object to serialize.</param>
        /// <param name="stream">The stream to serialize into.</param>
        /// <param name="name">The name of the serializer.</param>
        public static void ToXml<T>(this T item, Stream stream, string name = "default")
            => ToXml(item, typeof(T), stream, name);

        /// <summary>
        /// Serializes an object of <paramref name="type"/> into a XML string.
        /// </summary>
        /// <param name="item">The object to serialize.</param>
        /// <param name="type">The type of the object being serialized.</param>
        /// <param name="stream">The stream to serialize into.</param>
        /// <param name="name">The name of the serializer.</param>
        public static void ToXml(this object item, Type type, Stream stream, string name = "default")
            => _xmlSerializers.Value[name].SerializeToStream(stream, item, type);

        /// <summary>
        /// Deserializes a JSON string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized into.</typeparam>
        /// <param name="json">The JSON string to deserialize</param>
        /// <param name="name">The name of the serializer</param>
        /// <returns>An object of type T</returns>
        public static T FromJson<T>(this string json, string name = "default") where T : class
            => FromJson(json, typeof(T), name) as T;

        /// <summary>
        /// Deserializes a JSON string into an object of <paramref name="type"/>.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="type">The type of the object to be deserialized into.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>An object of given <paramref name="type"/></returns>
        public static object FromJson(this string json, Type type, string name = "default")
            => _jsonSerializers.Value[name].DeserializeFromString(json, type);

        /// <summary>
        /// Deserializes a JSON stream into an object of type T.
        /// </summary>
        /// <typeparam name="T">TThe type of the object to be deserialized into.</typeparam>
        /// <param name="stream">The stream to read the JSON from.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>An object of type T.</returns>
        public static T FromJson<T>(this Stream stream, string name = "default") where T : class
            => FromJson(stream, typeof(T), name) as T;

        /// <summary>
        /// Deserializes a JSON stream into an object of <paramref name="type"/>.
        /// </summary>
        /// <param name="stream">The stream to read the JSON from.</param>
        /// <param name="type">The type of the object to be deserialized into.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>An object of given <paramref name="type"/>.</returns>
        public static object FromJson(this Stream stream, Type type, string name = "default")
            => _jsonSerializers.Value[name].DeserializeFromStream(stream, type);

        /// <summary>
        /// Deserializes an XML string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized into.</typeparam>
        /// <param name="xml">The XML string to deserialize</param>
        /// <param name="name">The name of the serializer</param>
        /// <returns>An object of type T</returns>
        public static T FromXml<T>(this string xml, string name = "default") where T : class
            => FromXml(xml, typeof(T), name) as T;

        /// <summary>
        /// Deserializes an XML string into an object of <paramref name="type"/>.
        /// </summary>
        /// <param name="xml">The XML string to deserialize.</param>
        /// <param name="type">The type of the object to be deserialized into.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>An object of given <paramref name="type"/></returns>
        public static object FromXml(this string xml, Type type, string name = "default")
            => _xmlSerializers.Value[name].DeserializeFromString(xml, type);

        /// <summary>
        /// Deserializes an XML stream into an object of type T.
        /// </summary>
        /// <typeparam name="T">TThe type of the object to be deserialized into.</typeparam>
        /// <param name="stream">The stream to read the XML from.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>An object of type T.</returns>
        public static T FromXml<T>(this Stream stream, string name = "default") where T : class
            => FromXml(stream, typeof(T), name) as T;

        /// <summary>
        /// Deserializes an XML stream into an object of <paramref name="type"/>.
        /// </summary>
        /// <param name="stream">The stream to read the XML from.</param>
        /// <param name="type">The type of the object to be deserialized into.</param>
        /// <param name="name">The name of the serializer.</param>
        /// <returns>An object of given <paramref name="type"/>.</returns>
        public static object FromXml(this Stream stream, Type type, string name = "default")
            => _xmlSerializers.Value[name].DeserializeFromStream(stream, type);
    }
}
