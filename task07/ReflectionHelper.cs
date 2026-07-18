using System;
using System.Reflection;
using System.Text;

namespace task07
{
    public static class ReflectionHelper
    {
        public static string PrintTypeInfo(Type type)
        {
            var builder = new StringBuilder();

            var classDisplayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (classDisplayNameAttr != null)
            {
                builder.AppendLine($"Отображаемое имя: {classDisplayNameAttr.DisplayName}");
            }

            var classVersionAttr = type.GetCustomAttribute<VersionAttribute>();
            if (classVersionAttr != null)
            {
                builder.AppendLine($"Версия: {classVersionAttr.Major}.{classVersionAttr.Minor}");
            }

            var typeProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in typeProperties)
            {
                var propAttr = property.GetCustomAttribute<DisplayNameAttribute>();
                if (propAttr != null)
                {
                    builder.AppendLine($"{property.Name}: {propAttr.DisplayName}");
                }
            }

            var typeMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in typeMethods)
            {
                if (method.IsSpecialName) continue;

                var methodAttr = method.GetCustomAttribute<DisplayNameAttribute>();
                if (methodAttr != null)
                {
                    builder.AppendLine($"{method.Name}: {methodAttr.DisplayName}");
                }
            }

            return builder.ToString();
        }
    }
}