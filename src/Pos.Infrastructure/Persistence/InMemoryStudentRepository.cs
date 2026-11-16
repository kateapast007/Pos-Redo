using Pos.Domain.Students;

namespace Pos.Infrastructure.Persistence;

public sealed class InMemoryStudentRepository : IStudentRepository
{
    private readonly Dictionary<StudentId, Student> _items = new();
    private int _nextId = 1;

    public StudentId NextId() => new(_nextId++);

    public Student? Find(StudentId id) => _items.GetValueOrDefault(id);

    public IReadOnlyList<Student> GetAll() => _items.Values.ToArray();

    public void Save(Student student) => _items[student.Id] = student;
}
