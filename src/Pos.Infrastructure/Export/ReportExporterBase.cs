using System.Text;
using Pos.Application.Dtos;
using Pos.Application.Exceptions;
using Pos.Application.Ports;

namespace Pos.Infrastructure.Export;

public abstract class ReportExporterBase : IReportExporter
{
    public abstract string Format { get; }

    public string Export(IReadOnlyList<FinalGradeDto> grades, string path)
    {
        ArgumentNullException.ThrowIfNull(grades);
        if (string.IsNullOrWhiteSpace(path)) throw new UseCaseException("Путь к файлу не может быть пустым.");
        var rows = grades.OrderBy(g => g.StudentName, StringComparer.CurrentCultureIgnoreCase).ToArray();
        return Write(path.Trim(), Render(rows));
    }

    protected abstract string Render(IReadOnlyList<FinalGradeDto> rows);

    private string Write(string path, string content)
    {
        try
        {
            var fullPath = Path.GetFullPath(Path.HasExtension(path) ? path : $"{path}.{Format}");
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(fullPath, content, new UTF8Encoding(true));
            return fullPath;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            throw new UseCaseException($"Не удалось сохранить файл: {ex.Message}");
        }
    }
}
