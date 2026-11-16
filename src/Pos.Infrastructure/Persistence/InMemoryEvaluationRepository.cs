using Pos.Domain.Evaluations;
using Pos.Domain.Students;

namespace Pos.Infrastructure.Persistence;

public sealed class InMemoryEvaluationRepository : IEvaluationRepository
{
    private readonly Dictionary<EvaluationKey, Evaluation> _items = new();

    public Evaluation? Find(EvaluationKey key) => _items.GetValueOrDefault(key);

    public IReadOnlyList<Evaluation> GetAll() => _items.Values.ToArray();

    public IReadOnlyList<Evaluation> GetByStudent(StudentId studentId) =>
        _items.Values.Where(e => e.StudentId == studentId).ToArray();

    public void Save(Evaluation evaluation) => _items[evaluation.Id] = evaluation;

    public void Remove(EvaluationKey key) => _items.Remove(key);
}
