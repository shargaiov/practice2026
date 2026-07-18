using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public interface ICommand
{
    void Execute();
}

public static class ExceptionHandler
{
    public static Action<Exception, ICommand>? OnError { get; set; }
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _thread;
    private Action _activeStrategy;
    private bool _isWorking = true;

    public ServerThread()
    {
        _activeStrategy = ExecuteNormalMode;
        _thread = new Thread(WorkerLoop)
        {
            Name = "CommandProcessorThread",
            IsBackground = true
        };
    }

    public void Start() => _thread.Start();

    public void Enqueue(ICommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (!_queue.IsAddingCompleted)
        {
            try { _queue.Add(command); }
            catch (InvalidOperationException) { }
        }
    }

    public void AwaitTermination(int millisecondsTimeout = Timeout.Infinite)
    {
        _thread.Join(millisecondsTimeout);
    }

    public bool IsRunning => _thread.IsAlive;

    public void SetProcessingStrategy(Action newStrategy)
    {
        _activeStrategy = newStrategy;
    }

    public void InitiateHardStop()
    {
        EnsureExecutedOnServerThread();
        _isWorking = false;
    }

    public void InitiateSoftStop()
    {
        EnsureExecutedOnServerThread();
        _queue.CompleteAdding();
        SetProcessingStrategy(ExecuteDrainingMode);
    }

    private void WorkerLoop()
    {
        while (_isWorking)
        {
            _activeStrategy();
        }
    }

    private void ExecuteNormalMode()
    {
        ICommand? currentCommand = null;
        try
        {
            currentCommand = _queue.Take();
            currentCommand.Execute();
        }
        catch (InvalidOperationException)
        {
            _isWorking = false;
        }
        catch (Exception ex) when (currentCommand != null)
        {
            ExceptionHandler.OnError?.Invoke(ex, currentCommand);
        }
    }

    private void ExecuteDrainingMode()
    {
        ICommand? currentCommand = null;
        try
        {
            if (_queue.TryTake(out currentCommand))
            {
                currentCommand.Execute();
            }
            else
            {
                _isWorking = false;
            }
        }
        catch (Exception ex) when (currentCommand != null)
        {
            ExceptionHandler.OnError?.Invoke(ex, currentCommand);
        }
    }

    private void EnsureExecutedOnServerThread()
    {
        if (Thread.CurrentThread.ManagedThreadId != _thread.ManagedThreadId)
        {
            throw new InvalidOperationException("Команды остановки можно вызывать только из серверного потока.");
        }
    }
}