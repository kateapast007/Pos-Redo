namespace Pos.Domain.Students;

public interface IStudentRepository
{
    StudentId NextId();
    Student? Find(StudentId id);
    IReadOnlyList<Student> GetAll();
    void Save(Student student);
}
