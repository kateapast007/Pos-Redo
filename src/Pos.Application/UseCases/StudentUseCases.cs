using Pos.Application.Dtos;
using Pos.Domain.Students;

namespace Pos.Application.UseCases;

public sealed class StudentUseCases(IStudentRepository students)
{
    public StudentDto Add(string fullName)
    {
        var student = Student.Create(students.NextId(), fullName);
        students.Save(student);
        return Mapper.ToDto(student);
    }

    public IReadOnlyList<StudentDto> List() =>
        students.GetAll().OrderBy(s => s.Id.Value).Select(s => Mapper.ToDto(s)).ToArray();
}
