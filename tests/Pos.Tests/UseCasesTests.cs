using Xunit;
using Pos.Application.Dtos;
using Pos.Application.Exceptions;
using Pos.Application.Ports;
using Pos.Application.UseCases;
using Pos.Domain.Common;
using Pos.Domain.Evaluations;
using Pos.Domain.Grading;
using Pos.Infrastructure.Persistence;

namespace Pos.Tests;

public sealed class UseCasesTests
{
    private sealed class ExporterStub : IReportExporter
    {
        public IReadOnlyList<FinalGradeDto>? Received { get; private set; }

        public string Format => "stub";

        public string Export(IReadOnlyList<FinalGradeDto> grades, string path)
        {
            Received = grades;
            return path;
        }
    }

    private readonly ExporterStub _exporter = new();
    private readonly StudentUseCases _students;
    private readonly RubricUseCases _rubric;
    private readonly EvaluationUseCases _evaluations;
    private readonly ReportUseCases _reports;

    public UseCasesTests()
    {
        var studentRepository = new InMemoryStudentRepository();
        var rubricRepository = new InMemoryRubricRepository();
        var evaluationRepository = new InMemoryEvaluationRepository();
        _students = new StudentUseCases(studentRepository);
        _rubric = new RubricUseCases(rubricRepository);
        _evaluations = new EvaluationUseCases(studentRepository, rubricRepository, evaluationRepository, new EvaluationFactory());
        _reports = new ReportUseCases(studentRepository, rubricRepository, evaluationRepository, new FinalGradeCalculator(), [_exporter]);
    }

    [Fact]
    public void Grade_ScoreAboveMax_Throws()
    {
        var student = _students.Add("Иванов Иван");
        var criterion = _rubric.AddCriterion("Домашки", 10, 1);

        Assert.Throws<DomainException>(() => _evaluations.Grade(student.Id, criterion.Id, 12));
        Assert.Empty(_evaluations.List());
    }

    [Fact]
    public void Grade_UnknownCriterion_Throws()
    {
        var student = _students.Add("Иванов Иван");

        Assert.Throws<DomainException>(() => _evaluations.Grade(student.Id, 777, 5));
    }

    [Fact]
    public void Grade_UnknownStudent_Throws()
    {
        var criterion = _rubric.AddCriterion("Домашки", 10, 1);

        Assert.Throws<UseCaseException>(() => _evaluations.Grade(777, criterion.Id, 5));
    }

    [Fact]
    public void Grade_Twice_Throws()
    {
        var student = _students.Add("Иванов Иван");
        var criterion = _rubric.AddCriterion("Домашки", 10, 1);
        _evaluations.Grade(student.Id, criterion.Id, 5);

        Assert.Throws<UseCaseException>(() => _evaluations.Grade(student.Id, criterion.Id, 6));
    }

    [Fact]
    public void CorrectAndCancel_Work()
    {
        var student = _students.Add("Иванов Иван");
        var criterion = _rubric.AddCriterion("Домашки", 10, 1);
        _evaluations.Grade(student.Id, criterion.Id, 5);

        Assert.Equal(8m, _evaluations.Correct(student.Id, criterion.Id, 8).Score);

        _evaluations.Cancel(student.Id, criterion.Id);
        Assert.Empty(_evaluations.List());
    }

    [Fact]
    public void Final_AfterCorrection_IsRecalculated()
    {
        var student = _students.Add("Иванов Иван");
        var criterion = _rubric.AddCriterion("Домашки", 10, 1);
        _evaluations.Grade(student.Id, criterion.Id, 5);
        Assert.Equal(5m, _reports.Calculate(student.Id).Grade);

        _evaluations.Correct(student.Id, criterion.Id, 9);

        Assert.Equal(9m, _reports.Calculate(student.Id).Grade);
    }

    [Fact]
    public void Export_PassesCalculatedFinalsToExporter()
    {
        var student = _students.Add("Иванов Иван");
        var criterion = _rubric.AddCriterion("Домашки", 10, 1);
        _evaluations.Grade(student.Id, criterion.Id, 7);

        _reports.Export("stub", "any");

        Assert.Equal(7m, Assert.Single(_exporter.Received!).Grade);
    }

    [Fact]
    public void Export_UnknownFormat_Throws()
    {
        Assert.Throws<UseCaseException>(() => _reports.Export("xml", "any"));
    }
}
