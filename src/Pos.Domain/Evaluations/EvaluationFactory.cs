using Pos.Domain.Common;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Domain.Evaluations;

public sealed class EvaluationFactory
{
    public Evaluation Create(StudentId studentId, Rubric rubric, CriterionId criterionId, decimal score)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        var criterion = rubric.GetCriterion(criterionId);
        var value = new Score(score);
        criterion.EnsureAccepts(value);
        return new Evaluation(new EvaluationKey(studentId, criterionId), value);
    }
}
