using System;
using System.Collections.Generic;

namespace task19;

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
    public ICommand GetNext() => _tasks.Dequeue();
    public void Schedule(ICommand command) => _tasks.Enqueue(command);
}