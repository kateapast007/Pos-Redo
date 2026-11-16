using Pos.Domain.Common;

namespace Pos.Domain.Students;

public sealed class Student : Entity<StudentId>
{
    public string FullName { get; }

    private Student(StudentId id, string fullName) : base(id)
    {
        if (string.IsNullOrWhiteSpace(fullName)) throw new DomainException("Имя студента не может быть пустым.");
        FullName = fullName.Trim();
    }

    public static Student Create(StudentId id, string fullName) => new(id, fullName);
}
