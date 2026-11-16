using Pos.Domain.Common;

namespace Pos.Domain.Rubrics;

public sealed class Rubric
{
    private readonly List<Criterion> _criteria = [];

    public IReadOnlyList<Criterion> Criteria => _criteria;

    public decimal TotalWeight => _criteria.Sum(c => c.Weight.Value);

    public Criterion AddCriterion(string name, decimal maxScore, decimal weight)
    {
        var criterion = new Criterion(new CriterionId(_criteria.Count + 1), name, new Score(maxScore), new Weight(weight));
        _criteria.Add(criterion);
        return criterion;
    }

    public Criterion GetCriterion(CriterionId id) =>
        _criteria.FirstOrDefault(c => c.Id == id) ?? throw new DomainException($"Критерия №{id.Value} нет в рубрикаторе.");
}
