using Microsoft.Extensions.DependencyInjection;
using Pos.Application;
using Pos.Application.Exceptions;
using Pos.Domain.Common;

namespace Pos.App;

public static class DemoRunner
{
    public static void Run()
    {
        Console.WriteLine("\n========== ДЕМОНСТРАЦИЯ ==========");
        using var provider = CompositionRoot.Build();
        var pos = provider.GetRequiredService<PosFacade>();

        var homework = pos.AddCriterion("Домашние задания", 10, 0.4m);
        var test = pos.AddCriterion("Контрольная", 10, 0.4m);
        var seminars = pos.AddCriterion("Работа на семинарах", 5, 0.2m);
        Console.WriteLine("\nКритерии:");
        ConsoleView.Criteria(pos.ListCriteria());

        var ivan = pos.AddStudent("Иванов Иван");
        var maria = pos.AddStudent("Петрова Мария");
        Console.WriteLine("\nСтуденты:");
        ConsoleView.Students(pos.ListStudents());

        pos.GradeStudent(ivan.Id, homework.Id, 9);
        pos.GradeStudent(ivan.Id, test.Id, 7.5m);
        pos.GradeStudent(ivan.Id, seminars.Id, 5);
        pos.GradeStudent(maria.Id, homework.Id, 10);
        pos.GradeStudent(maria.Id, test.Id, 6);
        Console.WriteLine("\nОценки:");
        ConsoleView.Evaluations(pos.ListGrades());

        Console.WriteLine("\nПопытки нарушить правила:");
        TryShow("балл 12 при максимуме 5", () => pos.GradeStudent(maria.Id, seminars.Id, 12));
        TryShow("критерия нет в рубрикаторе", () => pos.GradeStudent(maria.Id, 99, 3));

        pos.CorrectGrade(maria.Id, test.Id, 8);
        pos.GradeStudent(maria.Id, seminars.Id, 4);
        Console.WriteLine("\nИтоги:");
        ConsoleView.Finals(pos.CalculateAllFinals());

        Console.WriteLine("\nЭкспорт итога:");
        foreach (var format in pos.ExportFormats)
            Console.WriteLine($"  {format}: {pos.ExportFinals(format, Path.Combine("exports", "demo-finals"))}");

        Console.WriteLine("\nДемонстрация завершена. Рабочее хранилище будет создано отдельно.");
        Console.WriteLine("===================================\n");
    }

    private static void TryShow(string what, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"  [!] {what}: действие почему-то выполнилось.");
        }
        catch (Exception ex) when (ex is DomainException or UseCaseException)
        {
            Console.WriteLine($"  Отказ ({what}): {ex.Message}");
        }
    }
}
