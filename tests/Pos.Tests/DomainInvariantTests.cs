using Xunit;
using Pos.Domain.Common;
using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Tests;

public sealed class DomainInvariantTests
{
    private static readonly StudentId StudentId = new(1);
    private readonly EvaluationFactory _factory = new();

    [Fact]
    public void Score_Negative_Throws()
    {
        Assert.Throws<DomainException>(() => new Score(-0.5m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Weight_NotPositive_Throws(int value)
    {
        Assert.Throws<DomainException>(() => new Weight(value));
    }

    [Fact]
    public void Student_EmptyName_Throws()
    {
        Assert.Throws<DomainException>(() => Student.Create(new StudentId(1), "  "));
    }

    [Fact]
    public void Rubric_ZeroMaxScore_Throws()
    {
        var rubric = new Rubric();
        Assert.Throws<DomainException>(() => rubric.AddCriterion("Тест", 0, 1));
        Assert.Empty(rubric.Criteria);
    }

    [Fact]
    public void Rubric_UnknownCriterion_Throws()
    {
        Assert.Throws<DomainException>(() => new Rubric().GetCriterion(new CriterionId(42)));
    }

    [Fact]
    public void Factory_ValidScore_CreatesEvaluation()
    {
        var rubric = new Rubric();
        var criterion = rubric.AddCriterion("Домашки", 10, 1);

        var evaluation = _factory.Create(StudentId, rubric, criterion.Id, 8);

        Assert.Equal(8m, evaluation.Score.Value);
        Assert.Equal(StudentId, evaluation.StudentId);
    }

    [Fact]
    public void Factory_ScoreAboveMax_Throws()
    {
        var rubric = new Rubric();
        var criterion = rubric.AddCriterion("Домашки", 10, 1);
        Assert.Throws<DomainException>(() => _factory.Create(StudentId, rubric, criterion.Id, 12));
    }

    [Fact]
    public void Factory_NegativeScore_Throws()
    {
        var rubric = new Rubric();
        var criterion = rubric.AddCriterion("Домашки", 10, 1);
        Assert.Throws<DomainException>(() => _factory.Create(StudentId, rubric, criterion.Id, -1));
    }

    [Fact]
    public void Factory_CriterionNotInRubric_Throws()
    {
        var rubric = new Rubric();
        rubric.AddCriterion("Домашки", 10, 1);
        Assert.Throws<DomainException>(() => _factory.Create(StudentId, rubric, new CriterionId(99), 5));
    }

    [Fact]
    public void Correct_ValidScore_ChangesScore()
    {
        var rubric = new Rubric();
        var criterion = rubric.AddCriterion("Домашки", 10, 1);
        var evaluation = _factory.Create(StudentId, rubric, criterion.Id, 5);

        evaluation.Correct(criterion, 9);

        Assert.Equal(9m, evaluation.Score.Value);
    }

    [Fact]
    public void Correct_ScoreAboveMax_ThrowsAndKeepsOldScore()
    {
        var rubric = new Rubric();
        var criterion = rubric.AddCriterion("Домашки", 10, 1);
        var evaluation = _factory.Create(StudentId, rubric, criterion.Id, 5);

        Assert.Throws<DomainException>(() => evaluation.Correct(criterion, 11));
        Assert.Equal(5m, evaluation.Score.Value);
    }

    [Fact]
    public void Correct_WithOtherCriterion_Throws()
    {
        var rubric = new Rubric();
        var first = rubric.AddCriterion("Домашки", 10, 1);
        var second = rubric.AddCriterion("Тест", 10, 1);
        var evaluation = _factory.Create(StudentId, rubric, first.Id, 5);

        Assert.Throws<DomainException>(() => evaluation.Correct(second, 6));
    }
}
