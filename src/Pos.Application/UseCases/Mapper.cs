using Pos.Application.Dtos;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Application.UseCases;

internal static class Mapper
{
    public static StudentDto ToDto(Student student) => new(student.Id.Value, student.FullName);

    public static CriterionDto ToDto(Criterion criterion) =>
        new(criterion.Id.Value, criterion.Name, criterion.MaxScore.Value, criterion.Weight.Value);
}
