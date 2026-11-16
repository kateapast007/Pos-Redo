using Pos.Domain.Rubrics;

namespace Pos.Infrastructure.Persistence;

public sealed class InMemoryRubricRepository : IRubricRepository
{
    private Rubric _rubric = new();

    public Rubric Get() => _rubric;

    public void Save(Rubric rubric) => _rubric = rubric;
}
