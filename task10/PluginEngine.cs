using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task10
{
    public class PluginEngine
    {
        public List<Type> FoundPlugins { get; } = new();

        public void DiscoverPlugins(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;

            var dllFiles = Directory.GetFiles(directoryPath, "*.dll");

            foreach (var file in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var validPlugins = assembly.GetTypes()
                        .Where(t => t.IsClass 
                                 && !t.IsAbstract 
                                 && typeof(IPlugin).IsAssignableFrom(t) 
                                 && t.GetCustomAttribute<PluginLoadAttribute>() != null);

                    FoundPlugins.AddRange(validPlugins);
                }
                catch
                {

                }
            }
        }

        public void ExecuteAll()
        {
            var sortedPlugins = ResolveDependencies();

            foreach (var pluginType in sortedPlugins)
            {
                var instance = (IPlugin)Activator.CreateInstance(pluginType)!;
                instance.Execute();
            }
        }

        private List<Type> ResolveDependencies()
        {
            var executionOrder = new List<Type>();
            var loaded = new HashSet<Type>();
            var loading = new HashSet<Type>();

            void Visit(Type pluginType)
            {
                if (loaded.Contains(pluginType)) return;
                
                if (loading.Contains(pluginType))
                    throw new InvalidOperationException($"Найдена циклическая зависимость: {pluginType.Name}");

                loading.Add(pluginType);

                var attr = pluginType.GetCustomAttribute<PluginLoadAttribute>();
                if (attr != null && attr.DependsOn != null)
                {
                    foreach (var dependencyName in attr.DependsOn)
                    {
                        var targetDependency = FoundPlugins.FirstOrDefault(t => t.Name == dependencyName);
                        if (targetDependency != null)
                        {
                            Visit(targetDependency); 
                        }
                    }
                }

                loading.Remove(pluginType);
                loaded.Add(pluginType);
                executionOrder.Add(pluginType);
            }

            foreach (var plugin in FoundPlugins)
            {
                Visit(plugin);
            }

            return executionOrder;
        }
    }
}