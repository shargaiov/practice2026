using System;
using System.Linq;
using Xunit;
using Moq;
using task05;

namespace task05tests
{
    public class TestClass
    {
        public int PublicField;
        private string _privateField = string.Empty;
        public int Property { get; set; }

        public void Method() { }
        
        public int Add(int x, int y) { return x + y; } 
    }

    [Serializable]
    public class AttributedClass { }

    public class ClassAnalyzerTests
    {
        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods();

            Assert.Contains("Method", methods);
            Assert.Contains("Add", methods);
        }

        [Fact]
        public void GetMethodParams_ReturnsReturnTypeAndParameterNames()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methodParams = analyzer.GetMethodParams("Add").ToList();

            Assert.Contains("Int32", methodParams); 
            Assert.Contains("x", methodParams);     
            Assert.Contains("y", methodParams);     
        }

        [Fact]
        public void GetAllFields_IncludesPrivateAndPublicFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields();

            Assert.Contains("_privateField", fields);
            Assert.Contains("PublicField", fields);
        }

        [Fact]
        public void GetProperties_ReturnsCorrectProperties()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties();

            Assert.Contains("Property", properties);
        }

        [Fact]
        public void HasAttribute_ReturnsTrueIfAttributeExists()
        {
            var analyzer = new ClassAnalyzer(typeof(AttributedClass));
            
            Assert.True(analyzer.HasAttribute<SerializableAttribute>());
        }

        [Fact]
        public void HasAttribute_ReturnsFalseIfAttributeDoesNotExist()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            
            Assert.False(analyzer.HasAttribute<SerializableAttribute>());
        }
    }
}