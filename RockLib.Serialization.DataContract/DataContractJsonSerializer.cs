using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using SystemJson = System.Runtime.Serialization.Json;

namespace RockLib.Serialization.DataContract;

/// <summary>
/// A JSON implementation of the <see cref="ISerializer"/> interface using <see cref="DataContractJsonSerializer"/>/>.
/// </summary>
public class DataContractJsonSerializer : ISerializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataContractJsonSerializer"/> class.
    /// </summary>
    /// <param name="name">The name of the serializer, used to when selecting which serializer to use.</param>
    /// <param name="settings">Options for customizing the JsonSerializer.</param>
    public DataContractJsonSerializer(string name = "default", DataContractJsonSerializerSettings? settings = null) =>
        (Name, Settings) = (name ?? "default", settings);

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Gets the <see cref="DataContractJsonSerializerSettings"/> options.
    /// </summary>
    public DataContractJsonSerializerSettings? Settings { get; }

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
            ? new SystemJson.DataContractJsonSerializer(type)
            : new SystemJson.DataContractJsonSerializer(type, Settings);

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
            ? new SystemJson.DataContractJsonSerializer(type)
            : new SystemJson.DataContractJsonSerializer(type, Settings);

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
            ? new SystemJson.DataContractJsonSerializer(type)
            : new SystemJson.DataContractJsonSerializer(type, Settings);

        using var stream = new MemoryStream();
        serializer.WriteObject(stream, item);
        stream.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
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
            ? new SystemJson.DataContractJsonSerializer(type)
            : new SystemJson.DataContractJsonSerializer(type, Settings);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        return serializer.ReadObject(stream);
    }
}
