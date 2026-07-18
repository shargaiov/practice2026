using System;
using System.IO;
using System.Linq;
using Xunit;
using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests : IDisposable
{
    private readonly string _tempPath;

    public FileSystemCommandsTests()
    {
        _tempPath = Path.Combine(Path.GetTempPath(), $"TestEnv_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempPath))
        {
            Directory.Delete(_tempPath, true);
        }
    }

    [Fact]
    public void DirectorySizeCommand_ShouldComputeCorrectByteSize()
    {
        File.WriteAllText(Path.Combine(_tempPath, "1.txt"), "ABC");
        File.WriteAllText(Path.Combine(_tempPath, "2.txt"), "XYZW");

        var cmd = new DirectorySizeCommand(_tempPath);
        cmd.Execute();

        Assert.Equal(7, cmd.CalculatedSize);
    }

    [Fact]
    public void FindFilesCommand_ShouldFilterByPattern()
    {
        File.WriteAllText(Path.Combine(_tempPath, "script.js"), "console.log(1);");
        File.WriteAllText(Path.Combine(_tempPath, "style.css"), "body { }");
        File.WriteAllText(Path.Combine(_tempPath, "app.js"), "init();");

        var cmd = new FindFilesCommand(_tempPath, "*.js");
        cmd.Execute();

        Assert.Equal(2, cmd.MatchingFiles.Count());
        Assert.Contains(cmd.MatchingFiles, f => f.EndsWith("script.js"));
        Assert.Contains(cmd.MatchingFiles, f => f.EndsWith("app.js"));
    }

    [Fact]
    public void DirectorySizeCommand_WorksWithEmptyDirectory()
    {
        var cmd = new DirectorySizeCommand(_tempPath);
        cmd.Execute();

        Assert.Equal(0, cmd.CalculatedSize);
    }
}