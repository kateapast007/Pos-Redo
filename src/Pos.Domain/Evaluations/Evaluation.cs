using Pos.Domain.Common;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Domain.Evaluations;

public sealed class Evaluation : Entity<EvaluationKey>
{
    public StudentId StudentId => Id.StudentId;
    public CriterionId CriterionId => Id.CriterionId;
    public Score Score { get; private set; }

    internal Evaluation(EvaluationKey key, Score score) : base(key) => Score = score;

    public void Correct(Criterion criterion, decimal newScore)
    {
        if (criterion.Id != CriterionId) throw new DomainException("Оценку нельзя исправить по другому критерию.");
        var score = new Score(newScore);
        criterion.EnsureAccepts(score);
        Score = score;
    }
}
