using System;
using System.Collections.Generic;

namespace task19;

public class TestCommand : ILongCommand
{
    private readonly int _id;
    private int _counter = 0;
    private readonly List<int> _log; // Добавили поле для лога

    public bool IsCompleted => _counter >= 3;

    // Конструктор теперь принимает и ID, и список для лога
    public TestCommand(int id, List<int> log) 
    { 
        _id = id; 
        _log = log; 
    }

    public void Execute()
    {
        if (!IsCompleted)
        {
            _counter++;
            Console.WriteLine($"Поток {_id} вызов {_counter}");
            
            // Записываем ID в лог для графика
            lock (_log) 
            {
                _log.Add(_id);
            }
        }
    }
}