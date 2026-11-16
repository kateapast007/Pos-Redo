using System.Globalization;
using Pos.Application;
using Pos.Application.Exceptions;
using Pos.Domain.Common;

namespace Pos.App;

public sealed class ConsoleUI(PosFacade pos)
{
    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n========== ПОС: ОЦЕНКА СТУДЕНТОВ ==========\n1. Добавить студента\n2. Показать студентов\n3. Добавить критерий\n4. Показать критерии\n5. Выставить оценку\n6. Показать оценки\n7. Исправить оценку\n8. Отменить оценку\n9. Итог студента\n10. Итоги всех студентов\n11. Экспортировать итоги\n0. Выход");
            Console.Write("Выберите действие: ");
            var choice = Console.ReadLine();
            if (choice is null || choice.Trim() == "0") return;
            try
            {
                switch (choice.Trim())
                {
                    case "1": AddStudent(); break;
                    case "2": ConsoleView.Students(pos.ListStudents()); break;
                    case "3": AddCriterion(); break;
                    case "4": ConsoleView.Criteria(pos.ListCriteria()); break;
                    case "5": GradeStudent(); break;
                    case "6": ShowGrades(); break;
                    case "7": CorrectGrade(); break;
                    case "8": CancelGrade(); break;
                    case "9": ShowFinal(); break;
                    case "10": ConsoleView.Finals(pos.CalculateAllFinals()); break;
                    case "11": Export(); break;
                    default: Console.WriteLine("Неизвестный пункт меню."); break;
                }
            }
            catch (DomainException ex) { Console.WriteLine($"Нарушено правило: {ex.Message}"); }
            catch (UseCaseException ex) { Console.WriteLine($"Операция не выполнена: {ex.Message}"); }
        }
    }

    private void AddStudent()
    {
        var student = pos.AddStudent(ReadRequired("ФИО: "));
        Console.WriteLine($"Студент добавлен, номер {student.Id}.");
    }

    private void AddCriterion()
    {
        var criterion = pos.AddCriterion(ReadRequired("Название критерия: "), ReadDecimal("Максимальный балл: "), ReadDecimal("Вес (например, 0.3): "));
        Console.WriteLine($"Критерий добавлен, номер {criterion.Id}.");
    }

    private void GradeStudent()
    {
        var grade = pos.GradeStudent(AskStudent(), AskCriterion(), ReadDecimal("Балл: "));
        Console.WriteLine($"Выставлено: {grade.StudentName} | {grade.CriterionName} — {ConsoleView.Number(grade.Score)} из {ConsoleView.Number(grade.MaxScore)}.");
    }

    private void ShowGrades()
    {
        Console.Write("Номер студента (Enter — все оценки): ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) ConsoleView.Evaluations(pos.ListGrades());
        else if (int.TryParse(input, out var studentId)) ConsoleView.Evaluations(pos.ListGrades(studentId));
        else Console.WriteLine("Введите целое число.");
    }

    private void CorrectGrade()
    {
        var grade = pos.CorrectGrade(AskStudent(), AskCriterion(), ReadDecimal("Новый балл: "));
        Console.WriteLine($"Исправлено: {grade.StudentName} | {grade.CriterionName} — {ConsoleView.Number(grade.Score)} из {ConsoleView.Number(grade.MaxScore)}.");
    }

    private void CancelGrade()
    {
        pos.CancelGrade(AskStudent(), AskCriterion());
        Console.WriteLine("Оценка отменена.");
    }

    private void ShowFinal() => Console.WriteLine(ConsoleView.Final(pos.CalculateFinal(AskStudent())));

    private void Export()
    {
        var format = ReadRequired($"Формат ({string.Join("/", pos.ExportFormats)}): ");
        Console.Write("Путь к файлу (Enter — exports/finals): ");
        var path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path)) path = Path.Combine("exports", "finals");
        Console.WriteLine($"Итоги сохранены: {pos.ExportFinals(format, path)}");
    }

    private int AskStudent()
    {
        ConsoleView.Students(pos.ListStudents());
        return ReadInt("Номер студента: ");
    }

    private int AskCriterion()
    {
        ConsoleView.Criteria(pos.ListCriteria());
        return ReadInt("Номер критерия: ");
    }

    private static string ReadRequired(string prompt)
    {
        while (true)
        {
            var value = ReadLine(prompt);
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            Console.WriteLine("Значение не должно быть пустым.");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            if (int.TryParse(ReadLine(prompt), out var value)) return value;
            Console.WriteLine("Введите целое число.");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            var input = ReadLine(prompt).Trim().Replace(',', '.');
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)) return value;
            Console.WriteLine("Введите число, например 7.5.");
        }
    }

    private static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }
}
