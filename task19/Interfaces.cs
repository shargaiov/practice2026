namespace task19;

public interface ICommand { void Execute(); }

public interface ILongCommand : ICommand
{
    bool IsCompleted { get; }
}