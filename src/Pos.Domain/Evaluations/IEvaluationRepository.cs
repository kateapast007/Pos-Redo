using Pos.Domain.Students;

namespace Pos.Domain.Evaluations;

public interface IEvaluationRepository
{
    Evaluation? Find(EvaluationKey key);
    IReadOnlyList<Evaluation> GetAll();
    IReadOnlyList<Evaluation> GetByStudent(StudentId studentId);
    void Save(Evaluation evaluation);
    void Remove(EvaluationKey key);
}
