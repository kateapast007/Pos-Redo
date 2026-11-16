using Pos.Domain.Common;

namespace Pos.Domain.Rubrics;

public sealed class Criterion : Entity<CriterionId>
{
    public string Name { get; }
    public Score MaxScore { get; }
    public Weight Weight { get; }

    internal Criterion(CriterionId id, string name, Score maxScore, Weight weight) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Название критерия не может быть пустым.");
        if (maxScore.Value <= 0) throw new DomainException("Максимальный балл критерия должен быть больше нуля.");
        Name = name.Trim();
        MaxScore = maxScore;
        Weight = weight;
    }

    public void EnsureAccepts(Score score)
    {
        if (score.Value > MaxScore.Value)
            throw new DomainException($"Балл {score} выходит за пределы критерия «{Name}»: допустимо от 0 до {MaxScore}.");
    }
}
