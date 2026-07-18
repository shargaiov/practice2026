using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands
{
    [DisplayName("Команда вычисления размера каталога")]
    [Version(1, 0)]
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _targetPath;
        
        public long CalculatedSize { get; private set; }

        public DirectorySizeCommand(string targetPath)
        {
            _targetPath = targetPath;
        }

        public void Execute()
        {
            CalculatedSize = 0;

            if (Directory.Exists(_targetPath))
            {
                CalculatedSize = new DirectoryInfo(_targetPath)
                    .GetFiles("*", SearchOption.AllDirectories)
                    .Sum(file => file.Length);
            }
        }
    }
}