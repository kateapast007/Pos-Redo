using Pos.Application.Dtos;
using Pos.Application.UseCases;

namespace Pos.Application;

public sealed class PosFacade(
    StudentUseCases students,
    RubricUseCases rubric,
    EvaluationUseCases evaluations,
    ReportUseCases reports)
{
    public IReadOnlyCollection<string> ExportFormats => reports.ExportFormats;

    public StudentDto AddStudent(string fullName) => students.Add(fullName);

    public IReadOnlyList<StudentDto> ListStudents() => students.List();

    public CriterionDto AddCriterion(string name, decimal maxScore, decimal weight) => rubric.AddCriterion(name, maxScore, weight);

    public IReadOnlyList<CriterionDto> ListCriteria() => rubric.List();

    public EvaluationDto GradeStudent(int studentId, int criterionId, decimal score) => evaluations.Grade(studentId, criterionId, score);

    public EvaluationDto CorrectGrade(int studentId, int criterionId, decimal newScore) => evaluations.Correct(studentId, criterionId, newScore);

    public void CancelGrade(int studentId, int criterionId) => evaluations.Cancel(studentId, criterionId);

    public IReadOnlyList<EvaluationDto> ListGrades(int? studentId = null) => evaluations.List(studentId);

    public FinalGradeDto CalculateFinal(int studentId) => reports.Calculate(studentId);

    public IReadOnlyList<FinalGradeDto> CalculateAllFinals() => reports.CalculateAll();

    public string ExportFinals(string format, string path) => reports.Export(format, path);
}
