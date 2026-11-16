using System.Globalization;
using Pos.Application.Dtos;

namespace Pos.App;

internal static class ConsoleView
{
    public static void Students(IReadOnlyList<StudentDto> students)
    {
        foreach (var s in students) Console.WriteLine($"  {s.Id}. {s.FullName}");
        if (students.Count == 0) Console.WriteLine("  Студентов пока нет.");
    }

    public static void Criteria(IReadOnlyList<CriterionDto> criteria)
    {
        foreach (var c in criteria) Console.WriteLine($"  {c.Id}. {c.Name} | максимум {Number(c.MaxScore)} | вес {Number(c.Weight)}");
        if (criteria.Count == 0) Console.WriteLine("  Критериев пока нет.");
    }

    public static void Evaluations(IReadOnlyList<EvaluationDto> evaluations)
    {
        foreach (var e in evaluations)
            Console.WriteLine($"  {e.StudentName} | {e.CriterionName}: {Number(e.Score)} из {Number(e.MaxScore)}");
        if (evaluations.Count == 0) Console.WriteLine("  Оценок пока нет.");
    }

    public static void Finals(IReadOnlyList<FinalGradeDto> finals)
    {
        foreach (var f in finals) Console.WriteLine(Final(f));
        if (finals.Count == 0) Console.WriteLine("  Студентов пока нет.");
    }

    public static string Final(FinalGradeDto f) => $"  {f.StudentName} | итог {Number(f.Grade)} из 10";

    public static string Number(decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
