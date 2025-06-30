using CashFlow.Shared.Exceptions.Application;
using System.Text.Json;

namespace CashFlow.Shared.Serialization
{
    /// <summary>
    /// Provides standardized JSON serialization and deserialization with consistent configuration.
    /// Ensures camelCase formatting and handles common serialization edge cases.
    /// </summary>
    public class JsonSerializerService : IJsonSerializerService
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance with default JSON serialization options:
        /// - camelCase property naming
        /// - case-insensitive property matching
        /// - null value ignoring
        /// </summary>
        public JsonSerializerService()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Serializes an object to JSON string.
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>JSON string representation of the object.</returns>
        /// <exception cref="JsonSerializerException">Thrown when serialization fails.</exception>
        public string Serialize<T>(T obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, _options);
            }
            catch (ArgumentNullException ex)
            {
                throw new JsonSerializerException("An error occurred during serialization. The provided object is null.", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new JsonSerializerException("An error occurred during serialization. The provided object type is not supported.", ex);
            }
            catch (JsonException ex)
            {
                throw new JsonSerializerException("An error occurred during serialization. The object could not be converted to JSON.", ex);
            }
        }

        /// <summary>
        /// Deserializes JSON string to specified type.
        /// </summary>
        /// <typeparam name="T">Target type for deserialization.</typeparam>
        /// <param name="json">JSON string to deserialize.</param>
        /// <returns>Deserialized object or null.</returns>
        /// <exception cref="JsonSerializerException">Thrown when deserialization fails.</exception>
        public T? Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, _options);
            }
            catch (ArgumentNullException ex)
            {
                throw new JsonSerializerException("An error occurred during deserialization. The input JSON string is null.", ex);
            }
            catch (JsonException ex)
            {
                throw new JsonSerializerException("An error occurred during deserialization. The input string is not a valid JSON for the expected type.", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new JsonSerializerException("An error occurred during deserialization. The target type is not supported.", ex);
            }
        }

        /// <summary>
        /// Deserializes JSON string to specified runtime type.
        /// </summary>
        /// <param name="json">JSON string to deserialize.</param>
        /// <param name="type">Target type for deserialization.</param>
        /// <returns>Deserialized object or null.</returns>
        /// <exception cref="JsonSerializerException">Thrown when deserialization fails.</exception>
        public object? Deserialize(string json, Type type)
        {
            try
            {
                return JsonSerializer.Deserialize(json, type, _options);
            }
            catch (ArgumentNullException ex)
            {
                throw new JsonSerializerException("An error occurred during deserialization. The input JSON string or type is null.", ex);
            }
            catch (JsonException ex)
            {
                throw new JsonSerializerException("An error occurred during deserialization. The input string is not valid JSON for the provided type.", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new JsonSerializerException("An error occurred during deserialization. The provided type is not supported.", ex);
            }
        }
    }
}