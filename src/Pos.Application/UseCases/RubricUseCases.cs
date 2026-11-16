using Pos.Application.Dtos;
using Pos.Domain.Rubrics;

namespace Pos.Application.UseCases;

public sealed class RubricUseCases(IRubricRepository rubrics)
{
    public CriterionDto AddCriterion(string name, decimal maxScore, decimal weight)
    {
        var rubric = rubrics.Get();
        var criterion = rubric.AddCriterion(name, maxScore, weight);
        rubrics.Save(rubric);
        return Mapper.ToDto(criterion);
    }

    public IReadOnlyList<CriterionDto> List() => rubrics.Get().Criteria.Select(c => Mapper.ToDto(c)).ToArray();
}
