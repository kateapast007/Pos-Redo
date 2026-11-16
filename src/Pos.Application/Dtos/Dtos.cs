namespace Pos.Application.Dtos;

public sealed record StudentDto(int Id, string FullName);

public sealed record CriterionDto(int Id, string Name, decimal MaxScore, decimal Weight);

public sealed record EvaluationDto(int StudentId, string StudentName, int CriterionId, string CriterionName, decimal Score, decimal MaxScore);

public sealed record FinalGradeDto(int StudentId, string StudentName, decimal Grade);
