using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace task13;

public class StudentDataHandler
{
    private readonly JsonSerializerOptions _settings;

    public StudentDataHandler()
    {
        _settings = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };
        _settings.Converters.Add(new BirthDateJsonConverter());
    }

    public string ExportToJson(Student student) => JsonSerializer.Serialize(student, _settings);

    public Student ImportFromJson(string json)
    {
        var student = JsonSerializer.Deserialize<Student>(json, _settings) 
                      ?? throw new ArgumentException("Данные повреждены");
        PerformValidation(student);
        return student;
    }

    private void PerformValidation(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName)) throw new ArgumentException("Имя не заполнено.");
        if (string.IsNullOrWhiteSpace(student.LastName)) throw new ArgumentException("Фамилия не заполнена.");
        
        // Уникальная проверка:
        if (student.BirthDate > DateTime.Now.AddYears(-10)) 
            throw new ArgumentException("Студент должен быть старше 10 лет.");

        if (student.Grades == null || student.Grades.Count == 0) 
            throw new ArgumentException("Список предметов пуст.");

        foreach (var sub in student.Grades)
        {
            if (sub.Grade < 2 || sub.Grade > 5) 
                throw new ArgumentException($"Оценка {sub.Grade} вне диапазона 2-5.");
        }
    }
}