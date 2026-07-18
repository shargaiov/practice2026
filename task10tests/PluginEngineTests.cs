using System;
using System.IO;
using Xunit;
using task10;

namespace task10tests
{

    [PluginLoad]
    public class CoreDatabasePlugin : IPlugin
    {
        public void Execute() => Console.WriteLine("Init:CoreDatabase");
    }

    [PluginLoad("CoreDatabasePlugin")]
    public class AuthServicePlugin : IPlugin
    {
        public void Execute() => Console.WriteLine("Init:AuthService");
    }

    [PluginLoad("CoreDatabasePlugin", "AuthServicePlugin")]
    public class WebInterfacePlugin : IPlugin
    {
        public void Execute() => Console.WriteLine("Init:WebInterface");
    }


    public class PluginEngineTests : IDisposable
    {
        private readonly string _testDirPath;

        public PluginEngineTests()
        {
            _testDirPath = Path.Combine(Path.GetTempPath(), $"TestEnv_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirPath))
            {
                Directory.Delete(_testDirPath, true);
            }
        }

        [Fact]
        public void Engine_ShouldExecutePlugins_InTopologicalOrder()
        {
            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var engine = new PluginEngine();
            
            string currentAssemblyPath = Path.GetDirectoryName(typeof(PluginEngineTests).Assembly.Location)!;
            
            engine.DiscoverPlugins(currentAssemblyPath);
            engine.ExecuteAll();

            string result = consoleOutput.ToString();
            string[] lines = result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            Assert.Contains("Init:CoreDatabase", lines[0]);
            Assert.Contains("Init:AuthService", lines[1]);
            Assert.Contains("Init:WebInterface", lines[2]);
        }

        [Fact]
        public void Engine_ShouldNotFail_OnEmptyDirectory()
        {
            var engine = new PluginEngine();
            engine.DiscoverPlugins(_testDirPath);
            engine.ExecuteAll();

            Assert.Empty(engine.FoundPlugins);
        }
    }
}