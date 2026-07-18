using System;
using System.Collections.Generic;

namespace task18;

public interface ICommand
{
    void Execute();
}

public interface IQuantumTask : ICommand
{
    bool IsCompleted { get; }
}

public interface IScheduler
{
    bool HasTasks();
    ICommand GetNext();
    void Schedule(ICommand command);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _tasks = new();

    public bool HasTasks() => _tasks.Count > 0;

    public ICommand GetNext()
    {
        if (_tasks.Count == 0) throw new InvalidOperationException("Планировщик пуст");
        return _tasks.Dequeue();
    }

    public void Schedule(ICommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        _tasks.Enqueue(command);
    }
}