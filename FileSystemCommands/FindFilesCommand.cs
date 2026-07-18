using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands;

public class FindFilesCommand : ICommand
{
    private readonly string _targetPath;
    private readonly string _pattern;
    
    public IEnumerable<string> MatchingFiles { get; private set; } = Enumerable.Empty<string>();

    public FindFilesCommand(string targetPath, string pattern)
    {
        _targetPath = targetPath;
        _pattern = pattern;
    }

    public void Execute()
    {
        if (Directory.Exists(_targetPath))
        {
            MatchingFiles = new DirectoryInfo(_targetPath)
                .GetFiles(_pattern, SearchOption.AllDirectories)
                .Select(f => f.FullName)
                .ToList();
        }
    }
}