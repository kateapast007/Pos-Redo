using Pos.Application.Dtos;
using Pos.Application.Exceptions;
using Pos.Application.Ports;
using Pos.Domain.Evaluations;
using Pos.Domain.Grading;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Application.UseCases;

public sealed class ReportUseCases(
    IStudentRepository students,
    IRubricRepository rubrics,
    IEvaluationRepository evaluations,
    FinalGradeCalculator calculator,
    IEnumerable<IReportExporter> exporters)
{
    private readonly IReportExporter[] _exporters = exporters.ToArray();

    public IReadOnlyCollection<string> ExportFormats => _exporters.Select(e => e.Format).ToArray();

    public FinalGradeDto Calculate(int studentId)
    {
        var student = students.Find(new StudentId(studentId)) ?? throw new UseCaseException($"Студент №{studentId} не найден.");
        return Calculate(student, rubrics.Get());
    }

    public IReadOnlyList<FinalGradeDto> CalculateAll()
    {
        var rubric = rubrics.Get();
        return students.GetAll().OrderBy(s => s.Id.Value).Select(s => Calculate(s, rubric)).ToArray();
    }

    public string Export(string format, string path)
    {
        var exporter = _exporters.FirstOrDefault(e => string.Equals(e.Format, format, StringComparison.OrdinalIgnoreCase))
            ?? throw new UseCaseException($"Неизвестный формат «{format}». Доступно: {string.Join(", ", ExportFormats)}.");
        return exporter.Export(CalculateAll(), path);
    }

    private FinalGradeDto Calculate(Student student, Rubric rubric)
    {
        var grade = calculator.Calculate(student, rubric, evaluations.GetByStudent(student.Id));
        return new FinalGradeDto(student.Id.Value, student.FullName, grade.Value);
    }
}
