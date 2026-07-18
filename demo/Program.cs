using task19;
using ScottPlot;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

var scheduler = new RoundRobinScheduler();
var server = new ServerThread(scheduler);
var log = new List<int>(); 

// Теперь вызываем конструктор с двумя аргументами
for (int i = 1; i <= 5; i++)
{
    server.EnqueueNew(new TestCommand(i, log));
}

server.Start();
System.Threading.Thread.Sleep(1000); // Дадим чуть больше времени
server.Stop();

// Генерация графика
var plot = new Plot();
plot.Title("Сетка выполнения задач (Round Robin)");
plot.XLabel("Сквозной шаг выполнения");
plot.YLabel("ID задачи");

for (int id = 1; id <= 5; id++)
{
    var steps = new List<double>();
    for(int i = 0; i < log.Count; i++) 
        if(log[i] == id) steps.Add(i + 1);
    
    if (steps.Count > 0)
        plot.Add.Scatter(steps.ToArray(), Enumerable.Repeat((double)id, steps.Count).ToArray());
}

// СОХРАНЕНИЕ
string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "progress_chart.png");
plot.SavePng(outputPath, 800, 600);
File.WriteAllText("report.txt", "Отчет по выполнению:\n" + string.Join("\n", log.Select((id, i) => $"Шаг {i+1}: Задача {id}")));

Console.WriteLine($"Готово! Файлы лежат тут: {Directory.GetCurrentDirectory()}");