using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace RockLib.Serialization.DataContract;

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
    public DataContractXmlSerializer(string name = "default", DataContractSerializerSettings? settings = null) =>
        (Name, Settings) = (name ?? "default", settings);

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Gets the <see cref="DataContractSerializerSettings"/> settings.
    /// </summary>
    public DataContractSerializerSettings? Settings { get; }

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

        var serializer = Settings is null
            ? new DataContractSerializer(type)
            : new DataContractSerializer(type, Settings);

        serializer.WriteObject(stream, item);
    }

    /// <inheritdoc />
    public object? DeserializeFromStream(Stream stream, Type type)
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

        var serializer = Settings is null
            ? new DataContractSerializer(type)
            : new DataContractSerializer(type, Settings);

        return serializer.ReadObject(stream);
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

        var serializer = Settings is null
            ? new DataContractSerializer(type)
            : new DataContractSerializer(type, Settings);

        var builder = new StringBuilder();

        using (var xmlWriter = XmlWriter.Create(builder))
        {
            serializer.WriteObject(xmlWriter, item);
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public object? DeserializeFromString(string data, Type type)
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

        var serializer = Settings is null
            ? new DataContractSerializer(type)
            : new DataContractSerializer(type, Settings);

        using var stringReader = new StringReader(data);
        using var xmlReader = XmlReader.Create(stringReader);
        return serializer.ReadObject(xmlReader);
    }
}
