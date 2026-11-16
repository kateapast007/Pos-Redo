using Pos.Application.Dtos;
using Pos.Application.Exceptions;
using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Application.UseCases;

public sealed class EvaluationUseCases(
    IStudentRepository students,
    IRubricRepository rubrics,
    IEvaluationRepository evaluations,
    EvaluationFactory factory)
{
    public EvaluationDto Grade(int studentId, int criterionId, decimal score)
    {
        var student = GetStudent(studentId);
        var rubric = rubrics.Get();
        var key = new EvaluationKey(student.Id, new CriterionId(criterionId));
        if (evaluations.Find(key) is not null)
            throw new UseCaseException("Оценка по этому критерию уже выставлена. Используйте исправление.");
        var evaluation = factory.Create(student.Id, rubric, key.CriterionId, score);
        evaluations.Save(evaluation);
        return ToDto(evaluation, student, rubric);
    }

    public EvaluationDto Correct(int studentId, int criterionId, decimal newScore)
    {
        var student = GetStudent(studentId);
        var rubric = rubrics.Get();
        var key = new EvaluationKey(student.Id, new CriterionId(criterionId));
        var evaluation = evaluations.Find(key) ?? throw new UseCaseException("Такой оценки нет. Сначала выставьте её.");
        evaluation.Correct(rubric.GetCriterion(key.CriterionId), newScore);
        evaluations.Save(evaluation);
        return ToDto(evaluation, student, rubric);
    }

    public void Cancel(int studentId, int criterionId)
    {
        var student = GetStudent(studentId);
        var key = new EvaluationKey(student.Id, new CriterionId(criterionId));
        if (evaluations.Find(key) is null) throw new UseCaseException("Такой оценки нет.");
        evaluations.Remove(key);
    }

    public IReadOnlyList<EvaluationDto> List(int? studentId = null)
    {
        var rubric = rubrics.Get();
        var known = students.GetAll().ToDictionary(s => s.Id);
        var items = studentId is null ? evaluations.GetAll() : evaluations.GetByStudent(GetStudent(studentId.Value).Id);
        return items
            .OrderBy(e => e.StudentId.Value)
            .ThenBy(e => e.CriterionId.Value)
            .Select(e => ToDto(e, known[e.StudentId], rubric))
            .ToArray();
    }

    private static EvaluationDto ToDto(Evaluation evaluation, Student student, Rubric rubric)
    {
        var criterion = rubric.GetCriterion(evaluation.CriterionId);
        return new EvaluationDto(
            student.Id.Value, student.FullName, criterion.Id.Value, criterion.Name, evaluation.Score.Value, criterion.MaxScore.Value);
    }

    private Student GetStudent(int studentId) =>
        students.Find(new StudentId(studentId)) ?? throw new UseCaseException($"Студент №{studentId} не найден.");
}
