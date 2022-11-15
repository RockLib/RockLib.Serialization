using System;
using System.IO;
using XSerializer;

namespace RockLib.Serialization.XSerializer;

/// <summary>
/// An XML implementation of the <see cref="ISerializer"/> interface using <see cref="XmlSerializer"/>/>.
/// </summary>
public class XSerializerXmlSerializer : ISerializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XSerializerXmlSerializer"/> class.
    /// </summary>
    /// <param name="name">The name of the serializer, used to when selecting which serializer to use.</param>
    /// <param name="options">Options for customizing the XmlSerializer.</param>
    public XSerializerXmlSerializer(string name = "default", XmlSerializationOptions? options = null) =>
        (Name, Options) = (name ?? "default", options);

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Gets the <see cref="XmlSerializationOptions"/> options.
    /// </summary>
    public XmlSerializationOptions? Options { get; }

    /// <inheritdoc />
    public void SerializeToStream(Stream stream, object item, Type type)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(type);
#else
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
#endif

        var serializer = Options is null
            ? XmlSerializer.Create(type)
            : XmlSerializer.Create(type, Options);

        serializer.Serialize(stream, item);
    }

    /// <inheritdoc />
    public object DeserializeFromStream(Stream stream, Type type)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(type);
#else
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
#endif

        var serializer = Options is null
            ? XmlSerializer.Create(type)
            : XmlSerializer.Create(type, Options);

        return serializer.Deserialize(stream);
    }

    /// <inheritdoc />
    public string SerializeToString(object item, Type type)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(type);
#else
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
#endif

        var serializer = Options is null
            ? XmlSerializer.Create(type)
            : XmlSerializer.Create(type, Options);

        return serializer.Serialize(item);
    }

    /// <inheritdoc />
    public object DeserializeFromString(string data, Type type)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(type);
#else
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
#endif

        var serializer = Options is null
            ? XmlSerializer.Create(type)
            : XmlSerializer.Create(type, Options);

        return serializer.Deserialize(data);
    }
}
