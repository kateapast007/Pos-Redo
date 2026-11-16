using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Domain.Evaluations;

public readonly record struct EvaluationKey(StudentId StudentId, CriterionId CriterionId);
