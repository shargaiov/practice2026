using System;

namespace CommandLib
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor)]
    public class DisplayNameAttribute : Attribute
    {
        public string Name { get; }
        public DisplayNameAttribute(string name) => Name = name;
    }
}