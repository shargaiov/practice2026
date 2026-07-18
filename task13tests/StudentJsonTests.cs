using Xunit;
using task13;

namespace task13tests;

public class StudentJsonTests
{
    private readonly StudentDataHandler _handler = new();

    [Fact]
    public void Serialization_ShouldOmitNullFields()
    {
        var s = new Student { FirstName = "Артем", LastName = "Петров", BirthDate = new DateTime(2000, 1, 1) };
        string json = _handler.ExportToJson(s);
        Assert.DoesNotContain("AdditionalNotes", json);
    }

    [Fact]
    public void Validation_ShouldThrow_WhenAgeIsTooLow()
    {
        var s = new Student { 
            FirstName = "Вася", LastName = "Пупкин", 
            BirthDate = DateTime.Now.AddYears(-5),
            Grades = new List<Subject> { new() { Name = "Физра", Grade = 5 } }
        };
        string json = _handler.ExportToJson(s);
        Assert.Throws<ArgumentException>(() => _handler.ImportFromJson(json));
    }
    
    [Fact]
    public void Integration_ValidData_ShouldSerializeAndDeserialize()
    {
        var s = new Student { 
            FirstName = "Ольга", LastName = "Волкова", 
            BirthDate = new DateTime(1999, 10, 10),
            Grades = new List<Subject> { new() { Name = "История", Grade = 4 } }
        };
        
        var result = _handler.ImportFromJson(_handler.ExportToJson(s));
        Assert.Equal("Ольга", result.FirstName);
    }
}