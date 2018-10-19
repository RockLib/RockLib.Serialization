using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RockLib.Configuration;
using RockLib.Configuration.ObjectFactory;
using RockLib.Immutable;

namespace RockLib.Serialization
{
    public static class Serializer
    {
        private static readonly Semimutable<IReadOnlyList<ISerializer>> _jsonSerializers = new Semimutable<IReadOnlyList<ISerializer>>(LoadJsonSerializers);
        private static readonly Semimutable<IReadOnlyList<ISerializer>> _xmlSerializers = new Semimutable<IReadOnlyList<ISerializer>>(LoadXmlSerializers);

        private static IReadOnlyList<ISerializer> LoadJsonSerializers()
        {
            var serializers = Config.Root.GetSection("RockLib.Serialization:JsonSerializers").Create<IReadOnlyList<ISerializer>>();
            return serializers == null || serializers.Count == 0
                ? new List<ISerializer> { new DefaultJsonSerializer() }
                : serializers;
        }

        private static IReadOnlyList<ISerializer> LoadXmlSerializers()
        {
            var serializers = Config.Root.GetSection("RockLib.Serialization:XmlSerializers").Create<IReadOnlyList<ISerializer>>();
            return serializers == null || serializers.Count == 0
                ? new List<ISerializer> { new DefaultXmlSerializer() }
                : serializers;
        }

        public static void SetJsonSerializers(IReadOnlyList<ISerializer> serializers) => _jsonSerializers.Value = serializers;
        public static void SetXmlSerializers(IReadOnlyList<ISerializer> serializers) => _xmlSerializers.Value = serializers;

        public static string ToJson<T>(this T item, string name = "default") => ToJson(item, typeof(T), name);
        public static string ToJson(this object item, Type type, string name = "default")
        {
            var serializer = _jsonSerializers.Value.FirstOrDefault(s => s.Name == name);
            return serializer?.SerializeToString(item, type)
                   ?? throw new Exception(); //TODO: Figure out the exception to throw
        }

        public static void ToJson<T>(this T item, Stream stream, string name = "default") => ToJson(item, typeof(T), stream, name);
        public static void ToJson(this object item, Type type, Stream stream, string name = "default")
        {
            var serializer = _jsonSerializers.Value.FirstOrDefault(s => s.Name == name);
            if (serializer != null)
                serializer.SerializeToStream(stream, item, type);
            else
                throw new Exception(); // TODO: Figure out the exception to throw
        }

        public static string ToXml<T>(this T item, string name = "default") => ToXml(item, typeof(T), name);
        public static string ToXml(this object item, Type type, string name = "default")
        {
            var serializer = _xmlSerializers.Value.FirstOrDefault(s => s.Name == name);
            return serializer?.SerializeToString(item, type)
                   ?? throw new Exception(); //TODO: Figure out the exception to throw
        }

        public static void ToXml<T>(this T item, Stream stream, string name = "default") => ToXml(item, typeof(T), stream, name);
        public static void ToXml(this object item, Type type, Stream stream, string name = "default")
        {
            var serializer = _xmlSerializers.Value.FirstOrDefault(s => s.Name == name);
            if (serializer != null)
                serializer.SerializeToStream(stream, item, type);
            else
                throw new Exception(); // TODO: Figure out the exception to throw
        }

        public static T FromJson<T>(this string json, string name = "default") where T : class
            => FromJson(json, typeof(T), name) as T;
        public static object FromJson(this string json, Type type, string name = "default")
        {
            var serializer = _jsonSerializers.Value.FirstOrDefault(s => s.Name == name);
            return serializer?.DeserializeFromString(json, type)
                   ?? throw new Exception(); //TODO: Figure out the exception to throw
        }

        public static T FromJson<T>(this Stream stream, string name = "default") where T : class
            => FromJson(stream, typeof(T), name) as T;
        public static object FromJson(this Stream stream, Type type, string name = "default")
        {
            var serializer = _jsonSerializers.Value.FirstOrDefault(s => s.Name == name);
            if (serializer != null)
                return serializer.DeserializeFromStream(stream, type);
            throw new Exception(); // TODO: Figure out the exception to throw
        }

        public static T FromXml<T>(this string xml, string name = "default") where T : class
            => FromXml(xml, typeof(T), name) as T;
        public static object FromXml(this string xml, Type type, string name = "default")
        {
            var serializer = _xmlSerializers.Value.FirstOrDefault(s => s.Name == name);
            return serializer?.DeserializeFromString(xml, type)
                   ?? throw new Exception(); //TODO: Figure out the exception to throw
        }

        public static T FromXml<T>(this Stream stream, string name = "default") where T : class
            => FromXml(stream, typeof(T), name) as T;
        public static object FromXml(this Stream stream, Type type, string name = "default")
        {
            var serializer = _xmlSerializers.Value.FirstOrDefault(s => s.Name == name);
            if (serializer != null)
                return serializer.DeserializeFromStream(stream, type);
            throw new Exception(); // TODO: Figure out the exception to throw
        }
    }
}
