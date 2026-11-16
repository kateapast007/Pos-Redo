using Pos.Domain.Common;
using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Domain.Grading;

public sealed class FinalGradeCalculator
{
    public FinalGrade Calculate(Student student, Rubric rubric, IReadOnlyCollection<Evaluation> evaluations)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(rubric);
        ArgumentNullException.ThrowIfNull(evaluations);
        if (rubric.Criteria.Count == 0) throw new DomainException("Рубрикатор пуст: итог посчитать нельзя.");

        var byCriterion = new Dictionary<CriterionId, Evaluation>();
        foreach (var evaluation in evaluations)
        {
            if (evaluation.StudentId != student.Id) throw new DomainException("В расчёт попала оценка другого студента.");
            rubric.GetCriterion(evaluation.CriterionId).EnsureAccepts(evaluation.Score);
            if (!byCriterion.TryAdd(evaluation.CriterionId, evaluation))
                throw new DomainException("По одному критерию не может быть двух оценок.");
        }

        var weighted = rubric.Criteria.Sum(c =>
            byCriterion.TryGetValue(c.Id, out var e) ? e.Score.Value / c.MaxScore.Value * c.Weight.Value : 0m);
        var grade = weighted / rubric.TotalWeight * FinalGrade.MaxValue;
        return new FinalGrade(Math.Round(grade, 2, MidpointRounding.AwayFromZero));
    }
}
