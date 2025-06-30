namespace CashFlow.Shared.Serialization
{
    /// <summary>
    /// Defines a contract for JSON serialization and deserialization operations.
    /// </summary>
    public interface IJsonSerializerService
    {
        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        /// <exception cref="JsonSerializerException">Thrown when serialization fails.</exception>
        string Serialize<T>(T obj);

        /// <summary>
        /// Deserializes the JSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The target type for deserialization.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object, or null if deserialization fails.</returns>
        /// <exception cref="JsonSerializerException">Thrown when serialization fails.</exception>
        T? Deserialize<T>(string json);

        /// <summary>
        /// Deserializes the JSON string to an object of the specified runtime type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="type">The target runtime type for deserialization.</param>
        /// <returns>The deserialized object, or null if deserialization fails.</returns>
        /// <exception cref="JsonSerializerException">Thrown when serialization fails.</exception>
        object? Deserialize(string json, Type type);
    }
}