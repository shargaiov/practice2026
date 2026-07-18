using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CommandLib;

namespace CommandRunner;

class Program
{
    static void Main()
    {
        Console.WriteLine("Запуск системы плагинов");

        string workingDir = Path.Combine(Path.GetTempPath(), "Task08_Environment");
        Directory.CreateDirectory(workingDir);
        File.WriteAllText(Path.Combine(workingDir, "config.json"), "{ \"setting\": true }");
        File.WriteAllText(Path.Combine(workingDir, "data.json"), "{ \"value\": 42 }");
        File.WriteAllText(Path.Combine(workingDir, "readme.txt"), "Important text");

        try
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"[ОШИБКА] Библиотека не найдена: {dllPath}");
                return;
            }

            Assembly commandsAssembly = Assembly.LoadFrom(dllPath);
            Console.WriteLine("Библиотека команд успешно загружена.\n");

            Type? sizeType = commandsAssembly.GetType("FileSystemCommands.DirectorySizeCommand");
            if (sizeType != null)
            {
                ICommand sizeCmd = (ICommand)Activator.CreateInstance(sizeType, workingDir)!;
                sizeCmd.Execute();
                
                long size = (long)sizeType.GetProperty("CalculatedSize")!.GetValue(sizeCmd)!;
                Console.WriteLine($"Команда DirectorySizeCommand отработала. Размер папки: {size} байт.");
            }

            Type? findType = commandsAssembly.GetType("FileSystemCommands.FindFilesCommand");
            if (findType != null)
            {
                ICommand findCmd = (ICommand)Activator.CreateInstance(findType, workingDir, "*.json")!;
                findCmd.Execute();

                var files = (IEnumerable<string>)findType.GetProperty("MatchingFiles")!.GetValue(findCmd)!;
                Console.WriteLine($"Команда FindFilesCommand отработала. Найдены JSON файлы:");
                foreach (var file in files)
                {
                    Console.WriteLine($" ---> {Path.GetFileName(file)}");
                }
            }
        }
        finally
        {
            if (Directory.Exists(workingDir))
            {
                Directory.Delete(workingDir, true);
            }
        }
    }
}