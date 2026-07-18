using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task19;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _worker;
    public IScheduler Scheduler { get; }
    private bool _isActive = true;

    public ServerThread(IScheduler scheduler)
    {
        Scheduler = scheduler;
        _worker = new Thread(ProcessLoop) { IsBackground = true };
    }

    public void Start() => _worker.Start();
    
    public void EnqueueNew(ICommand cmd) => _queue.Add(cmd);

    public void Stop()
    {
        _isActive = false;
        _queue.CompleteAdding();
    }

    private void ProcessLoop()
    {
        while (_isActive)
        {
            try
            {
                int timeout = Scheduler.HasTasks() ? 0 : Timeout.Infinite;

                if (_queue.TryTake(out var cmd, timeout))
                {
                    ExecuteAndReschedule(cmd);
                }
                else if (Scheduler.HasTasks())
                {
                    ExecuteAndReschedule(Scheduler.GetNext());
                }
            }
            catch (Exception) { _isActive = false; }
        }
    }

    private void ExecuteAndReschedule(ICommand cmd)
    {
        cmd.Execute();
        if (cmd is ILongCommand longCmd && !longCmd.IsCompleted)
        {
            Scheduler.Schedule(cmd);
        }
    }
}