using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05
{
    public class ClassAnalyzer
    {
        private Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type;
        }

        public IEnumerable<string> GetPublicMethods() =>
            _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                 .Select(m => m.Name);

        public IEnumerable<string> GetMethodParams(string methodname)
        {
            var method = _type.GetMethod(methodname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            
            return method == null 
                ? Enumerable.Empty<string>() 
                : new[] { method.ReturnType.Name }.Concat(method.GetParameters().Select(p => p.Name));
        }
        public IEnumerable<string> GetAllFields() =>
            _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                 .Select(f => f.Name);

        public IEnumerable<string> GetProperties() =>
            _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                 .Select(p => p.Name);

        public bool HasAttribute<T>() where T : Attribute =>
            _type.GetCustomAttributes(typeof(T), true).Any();
    }
}