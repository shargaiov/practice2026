using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task18;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _worker;
    private bool _isRunning = true;

    public IScheduler Scheduler { get; }
    public bool IsActive => _worker.IsAlive;

    public ServerThread(IScheduler scheduler)
    {
        Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _worker = new Thread(ProcessLoop) { IsBackground = true, Name = "SchedulerThread" };
    }

    public void Start() => _worker.Start();
    public void WaitToFinish() => _worker.Join();

    public void EnqueueNew(ICommand cmd)
    {
        if (cmd == null) throw new ArgumentNullException(nameof(cmd));
        if (!_queue.IsAddingCompleted)
        {
            try { _queue.Add(cmd); }
            catch (InvalidOperationException) { }
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _queue.CompleteAdding();
    }

    private void ProcessLoop()
    {
        while (_isRunning)
        {
            ICommand? cmd = null;
            try
            {
                int timeout = Scheduler.HasTasks() ? 0 : Timeout.Infinite;

                if (_queue.TryTake(out cmd, timeout))
                {
                    ExecuteAndReschedule(cmd);
                }
                else if (Scheduler.HasTasks())
                {
                    cmd = Scheduler.GetNext();
                    ExecuteAndReschedule(cmd);
                }
            }
            catch (InvalidOperationException)
            {
                _isRunning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error]: {ex.Message}");
            }
        }
    }

    private void ExecuteAndReschedule(ICommand cmd)
    {
        cmd.Execute();
        
        if (cmd is IQuantumTask quantumTask && !quantumTask.IsCompleted)
        {
            Scheduler.Schedule(cmd);
        }
    }
}