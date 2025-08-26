using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace Clipo.Domain.Extensions
{
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<Enum, string> _descriptionCache = new();

        /// <summary>
        /// Retorna a string definida no atributo [Description] do valor do enum.
        /// Se o atributo não estiver presente, devolve o nome literal do valor.
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            if (_descriptionCache.TryGetValue(value, out string? cached))
                return cached;

            Type type = value.GetType();
            MemberInfo? member = type.GetMember(value.ToString()).FirstOrDefault();

            string description = member?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description
                ?? value.ToString();

            _descriptionCache.TryAdd(value, description);
            return description;
        }
    }
}
