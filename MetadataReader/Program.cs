using System;
using System.Linq;
using System.Reflection;

namespace MetadataReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("[ОШИБКА] Укажите путь к .dll файлу в качестве параметра командной строки.");
                return;
            }

            string libraryPath = args[0];

            try
            {
                Assembly targetAssembly = Assembly.LoadFrom(libraryPath);
                Console.WriteLine($"\n Анализ метаданных библиотеки: {targetAssembly.GetName().Name} \n");

                var allTypes = targetAssembly.GetTypes().Where(t => t.IsClass);

                foreach (var currentType in allTypes)
                {
                    Console.WriteLine($"[Класс] {currentType.FullName}");

                    var customAttributes = currentType.GetCustomAttributes();
                    if (customAttributes.Any())
                    {
                        Console.WriteLine("  Атрибуты:");
                        foreach (var attr in customAttributes)
                        {
                            Console.WriteLine($"    -> {attr.GetType().Name}");
                            if (attr.GetType().Name == "DisplayNameAttribute")
                                Console.WriteLine($"       Имя: {attr.GetType().GetProperty("Name")?.GetValue(attr)}");
                            else if (attr.GetType().Name == "VersionAttribute")
                                Console.WriteLine($"       Версия: {attr.GetType().GetProperty("Major")?.GetValue(attr)}.{attr.GetType().GetProperty("Minor")?.GetValue(attr)}");
                        }
                    }

                    var typeConstructors = currentType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (typeConstructors.Any())
                    {
                        Console.WriteLine("  Конструкторы:");
                        foreach (var constructor in typeConstructors)
                        {
                            var parameters = string.Join(", ", constructor.GetParameters()
                                .Select(p => $"{p.ParameterType.Name} {p.Name}"));
                            Console.WriteLine($"    -> {currentType.Name}({parameters})");
                        }
                    }

                    var typeMethods = currentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Where(m => !m.IsSpecialName && m.DeclaringType == currentType);
                    
                    if (typeMethods.Any())
                    {
                        Console.WriteLine("  Методы:");
                        foreach (var method in typeMethods)
                        {
                            var parameters = string.Join(", ", method.GetParameters()
                                .Select(p => $"{p.ParameterType.Name} {p.Name}"));
                            Console.WriteLine($"    -> {method.ReturnType.Name} {method.Name}({parameters})");
                        }
                    }

                    Console.WriteLine(new string('-', 50));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[КРИТИЧЕСКАЯ ОШИБКА] Не удалось проанализировать библиотеку: {ex.Message}");
            }
        }
    }
}