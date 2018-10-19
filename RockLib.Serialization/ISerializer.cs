using System;
using System.IO;

namespace RockLib.Serialization
{
    /// <summary>
    /// Defines a common interface for general-purpose serialization.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Gets the name of the <see cref="ISerializer"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Serialize the <paramref name="item"/> as <paramref name="type"/>
        /// to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="item">The object to serialize.</param>
        /// <param name="type">The type to serialize the object as.</param>
        void SerializeToStream(Stream stream, object item, Type type);

        /// <summary>
        /// Deserialize from the <paramref name="stream"/> as <paramref name="type"/>.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="type">The type to deserialize as.</param>
        /// <returns>An instance of type <paramref name="type"/>.</returns>
        object DeserializeFromStream(Stream stream, Type type);

        /// <summary>
        /// Serialize the <paramref name="item"/> as <paramref name="type"/> to a string.
        /// </summary>
        /// <param name="item">The object to serialize.</param>
        /// <param name="type">The type to serialize the object as.</param>
        /// <returns>A string representing the <paramref name="item"/> object.</returns>
        string SerializeToString(object item, Type type);

        /// <summary>
        /// Deserialize from the string as <paramref name="type"/>.
        /// </summary>
        /// <param name="data">The string containing a representation of an object of type <paramref name="type"/>.</param>
        /// <param name="type">The type to deserialize as.</param>
        /// <returns>An instance of type <paramref name="type"/>.</returns>
        object DeserializeFromString(string data, Type type);
    }
}
