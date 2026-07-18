using System;

namespace task10
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginLoadAttribute : Attribute
    {
        public string[] DependsOn { get; }

        public PluginLoadAttribute(params string[] dependsOn)
        {
            DependsOn = dependsOn;
        }
    }
}