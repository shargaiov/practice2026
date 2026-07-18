using task13;

var handler = new StudentDataHandler();
var student = new Student {
    FirstName = "Максим",
    LastName = "Козлов",
    BirthDate = new DateTime(2005, 5, 20),
    Grades = new List<Subject> { new() { Name = "География", Grade = 5 } }
};

string json = handler.ExportToJson(student);
Console.WriteLine(json);