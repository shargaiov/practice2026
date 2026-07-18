using System;
using Xunit;
using task17;

namespace task17tests;

public class ServerTests
{
    private const int TimeoutMs = 2000;

    private class TrackingCommand : ICommand
    {
        public bool HasExecuted { get; private set; }
        private readonly Action? _action;

        public TrackingCommand(Action? action = null) => _action = action;

        public void Execute()
        {
            HasExecuted = true;
            _action?.Invoke();
        }
    }

    [Fact]
    public void HardStop_ShouldInterruptProcessingImmediately()
    {
        var server = new ServerThread();
        var cmd1 = new TrackingCommand();
        var stopCmd = new HardStopCommand(server);
        var cmd2 = new TrackingCommand();

        server.Enqueue(cmd1);
        server.Enqueue(stopCmd);
        server.Enqueue(cmd2);

        server.Start();
        server.AwaitTermination(TimeoutMs);

        Assert.True(cmd1.HasExecuted);
        Assert.False(cmd2.HasExecuted);
        Assert.False(server.IsRunning);
    }

    [Fact]
    public void SoftStop_ShouldDrainQueueBeforeTerminating()
    {
        var server = new ServerThread();
        var cmd1 = new TrackingCommand();
        var stopCmd = new SoftStopCommand(server);
        var cmd2 = new TrackingCommand();

        server.Enqueue(cmd1);
        server.Enqueue(stopCmd);
        server.Enqueue(cmd2);

        server.Start();
        server.AwaitTermination(TimeoutMs);

        Assert.True(cmd1.HasExecuted);
        Assert.True(cmd2.HasExecuted);
        Assert.False(server.IsRunning);
    }

    [Fact]
    public void StopCommands_CalledFromOutside_ShouldThrowInvalidOperationException()
    {
        var server = new ServerThread();
        var hard = new HardStopCommand(server);
        var soft = new SoftStopCommand(server);

        var hardEx = Assert.Throws<InvalidOperationException>(() => hard.Execute());
        Assert.Contains("только из серверного потока", hardEx.Message);

        var softEx = Assert.Throws<InvalidOperationException>(() => soft.Execute());
        Assert.Contains("только из серверного потока", softEx.Message);
    }

    [Fact]
    public void ExceptionsInCommands_AreCaught_AndPassedToHandler()
    {
        var server = new ServerThread();
        var targetException = new ApplicationException("Crash Test");
        var badCommand = new TrackingCommand(() => throw targetException);
        
        Exception? caughtEx = null;
        ICommand? caughtCmd = null;

        ExceptionHandler.OnError = (ex, cmd) =>
        {
            caughtEx = ex;
            caughtCmd = cmd;
        };

        server.Enqueue(badCommand);
        server.Enqueue(new HardStopCommand(server));

        server.Start();
        server.AwaitTermination(TimeoutMs);

        Assert.Same(targetException, caughtEx);
        Assert.Same(badCommand, caughtCmd);
    }
}