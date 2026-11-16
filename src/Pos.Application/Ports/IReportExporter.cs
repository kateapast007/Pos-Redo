using Pos.Application.Dtos;

namespace Pos.Application.Ports;

public interface IReportExporter
{
    string Format { get; }
    string Export(IReadOnlyList<FinalGradeDto> grades, string path);
}
