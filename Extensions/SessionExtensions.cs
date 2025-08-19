using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace GPTCvAssistant.Extensions
{
    /// <summary>
    /// Extension methods for working with session storage
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Stores an object in session as JSON
        /// </summary>
        /// <typeparam name="T">Type of object to store</typeparam>
        /// <param name="session">HTTP session</param>
        /// <param name="key">Session key</param>
        /// <param name="value">Value to store</param>
        public static void SetObjectAsJson<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        /// <summary>
        /// Retrieves an object from session JSON
        /// </summary>
        /// <typeparam name="T">Type of object to retrieve</typeparam>
        /// <param name="session">HTTP session</param>
        /// <param name="key">Session key</param>
        /// <returns>Deserialized object or default value</returns>
        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }
    }
}