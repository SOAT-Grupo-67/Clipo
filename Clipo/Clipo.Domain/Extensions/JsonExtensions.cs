using System.Text.Json;

namespace Clipo.Domain.Extensions
{
    public static class JsonExtensions
    {
        // Opções padrão (compacta)
        private static readonly JsonSerializerOptions _compact =
            new(JsonSerializerDefaults.Web);

        // Opções com identação bonita (pretty-print)
        private static readonly JsonSerializerOptions _pretty =
            new(JsonSerializerDefaults.Web) { WriteIndented = true };

        /// <summary>
        /// Serializa o objeto em JSON.
        /// Defina <paramref name="indented"/> como <c>true</c> para pretty-print.
        /// </summary>
        public static string ToJson(this object obj, bool indented = false) =>
            JsonSerializer.Serialize(obj, indented ? _pretty : _compact);

        /// <summary>
        /// Desserializa a string JSON no tipo especificado.
        /// </summary>
        public static T? FromJson<T>(this string json, bool indented = false) =>
            JsonSerializer.Deserialize<T>(json, indented ? _pretty : _compact);
    }
}