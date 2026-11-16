using Xunit;
using Pos.Domain.Common;
using Pos.Domain.Evaluations;
using Pos.Domain.Grading;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Tests;

public sealed class FinalGradeCalculatorTests
{
    private readonly EvaluationFactory _factory = new();
    private readonly FinalGradeCalculator _calculator = new();
    private readonly Student _student = Student.Create(new StudentId(1), "Иванов Иван");

    [Fact]
    public void Calculate_AllGraded_ReturnsWeightedAverage()
    {
        var (rubric, homework, quiz) = CreateRubric();
        var evaluations = new[] { Grade(rubric, homework, 8), Grade(rubric, quiz, 5) };

        var result = _calculator.Calculate(_student, rubric, evaluations);

        Assert.Equal(8.5m, result.Value);
    }

    [Fact]
    public void Calculate_MissingEvaluation_CountsAsZero()
    {
        var (rubric, homework, _) = CreateRubric();

        var result = _calculator.Calculate(_student, rubric, [Grade(rubric, homework, 8)]);

        Assert.Equal(6m, result.Value);
    }

    [Fact]
    public void Calculate_FullMarks_GivesTen()
    {
        var (rubric, homework, quiz) = CreateRubric();

        var result = _calculator.Calculate(_student, rubric, [Grade(rubric, homework, 10), Grade(rubric, quiz, 5)]);

        Assert.Equal(10m, result.Value);
    }

    [Fact]
    public void Calculate_NoEvaluations_GivesZero()
    {
        var (rubric, _, _) = CreateRubric();

        Assert.Equal(0m, _calculator.Calculate(_student, rubric, []).Value);
    }

    [Fact]
    public void Calculate_EmptyRubric_Throws()
    {
        Assert.Throws<DomainException>(() => _calculator.Calculate(_student, new Rubric(), []));
    }

    [Fact]
    public void Calculate_EvaluationOfAnotherStudent_Throws()
    {
        var (rubric, homework, _) = CreateRubric();
        var foreign = _factory.Create(new StudentId(2), rubric, homework.Id, 8);

        Assert.Throws<DomainException>(() => _calculator.Calculate(_student, rubric, [foreign]));
    }

    [Fact]
    public void Calculate_EvaluationOutsideRubric_Throws()
    {
        var (rubric, _, quiz) = CreateRubric();
        var orphan = Grade(rubric, quiz, 3);
        var smallRubric = new Rubric();
        smallRubric.AddCriterion("Единственный", 10, 1);

        Assert.Throws<DomainException>(() => _calculator.Calculate(_student, smallRubric, [orphan]));
    }

    private static (Rubric Rubric, Criterion Homework, Criterion Quiz) CreateRubric()
    {
        var rubric = new Rubric();
        var homework = rubric.AddCriterion("Домашки", 10, 3);
        var quiz = rubric.AddCriterion("Тест", 5, 1);
        return (rubric, homework, quiz);
    }

    private Evaluation Grade(Rubric rubric, Criterion criterion, decimal score) =>
        _factory.Create(_student.Id, rubric, criterion.Id, score);
}
